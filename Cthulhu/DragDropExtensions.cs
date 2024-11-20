#if WINDOWS
using Microsoft.Maui.Platform;
public static class DragDropExtensions
{
    public static void RegisterDrop( this IElement element, IMauiContext? mauiContext, Func<string,Task>? content )
    {
        ArgumentNullException.ThrowIfNull( mauiContext );
        var view = element.ToPlatform( mauiContext );
        DragDropHelper.RegisterDrop( view, content );
    }
    public static void UnRegisterDrop( this IElement element, IMauiContext? mauiContext )
    {
        ArgumentNullException.ThrowIfNull( mauiContext );
        var view = element.ToPlatform( mauiContext );
        DragDropHelper.UnRegisterDrop( view );
    }
}
#endif
