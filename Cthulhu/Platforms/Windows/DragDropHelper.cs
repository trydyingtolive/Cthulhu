using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using DataPackageOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation;
using DragEventArgs = Microsoft.UI.Xaml.DragEventArgs;

public static class DragDropHelper
{
    public static void RegisterDrop( UIElement element, Func<string, Task>? content )
    {
        element.AllowDrop = true;
        element.Drop += async ( s, e ) =>
        {
            if ( e.DataView.Contains( StandardDataFormats.StorageItems ) )
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach ( var item in items )
                {
                    if ( item is StorageFile file )
                    {
                        if ( content is not null )
                        {
                            await content.Invoke( file.Path );
                        }
                    }
                }
            }
        };
        element.DragOver += OnDragOver;
    }


    public static void UnRegisterDrop( UIElement element )
    {
        element.AllowDrop = false;
        element.DragOver -= OnDragOver;
    }
    private static async void OnDragOver( object sender, DragEventArgs e )
    {
        if ( e.DataView.Contains( StandardDataFormats.StorageItems ) )
        {
            var deferral = e.GetDeferral();
            var extensions = new List<string> { ".mp4", ".mkv" };
            var isAllowed = false;
            var items = await e.DataView.GetStorageItemsAsync();
            foreach ( var item in items )
            {
                if ( item is StorageFile file && extensions.Contains( file.FileType ) )
                {
                    isAllowed = true;
                    break;
                }
            }
            e.AcceptedOperation = isAllowed ? DataPackageOperation.Copy : DataPackageOperation.None;
            deferral.Complete();
        }
        e.AcceptedOperation = DataPackageOperation.None;
    }

}
