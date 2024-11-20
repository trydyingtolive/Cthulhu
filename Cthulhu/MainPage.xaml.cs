using System.Diagnostics;

namespace Cthulhu;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

#if WINDOWS
        Loaded += ( sender, args ) =>
        {
            this.RegisterDrop( Handler?.MauiContext, async ( path ) =>
            {
                var ffmpeg = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "unmanaged/ffmpeg.exe" );
                var output = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Fixed" );


                var format = path.Split( "." ).Last();
                output += "." + format;

                if ( File.Exists( output ) )
                {
                    File.Delete( output );
                }

                ProcessStartInfo ProcessInfo;
                Process Process;

                ProcessInfo = new ProcessStartInfo( ffmpeg, $" -i \"{path}\" -c copy -sn \"{output}\"" );
                ProcessInfo.CreateNoWindow = true;
                ProcessInfo.UseShellExecute = false;

                Process = Process.Start( ProcessInfo );

                Process.WaitForExit();

                File.Delete( path );
                File.Copy( output, path );
                File.Delete( output );

            } );
        };

        Unloaded += ( sender, args ) =>
        {
            this.UnRegisterDrop( Handler?.MauiContext );
        };
#endif
    }

}


