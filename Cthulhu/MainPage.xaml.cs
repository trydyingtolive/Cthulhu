using System.Collections.Concurrent;
using System.Diagnostics;
using Windows.Media.Effects;
using Windows.Storage.Search;

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
                Task.Run(() => EnqueVideo( path ));
            } );
        };

        Unloaded += ( sender, args ) =>
        {
            this.UnRegisterDrop( Handler?.MauiContext );
        };
#endif
    }

    bool isRunning;
    bool errored= false;
    ConcurrentQueue<string> paths = new ConcurrentQueue<string>();

    public async Task EnqueVideo( string path )
    {
        await Task.Delay( 10 );
        paths.Enqueue( path );

        if ( paths.Count > 1 )
        {
            MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = $"{paths.Count} are sacrificed"; } );
        }
        else
        {
            MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = "The beast eats..."; } );
        }

        await Task.Delay( 10 );

        if ( !isRunning )
        {
            isRunning = true;
            await ProcessVideos();
        }
    }

    public async Task ProcessVideos()
    {
        while ( paths.TryDequeue( out string? path ) )
        {
            await ProcessVideo( path );

            if ( errored )
            {
                return;
            }

            if ( paths.Count > 1 )
            {
                MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = $"{paths.Count} are sacrificed"; } );
            }
            else
            {
                MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = "The beast eats..."; } );
            }
            await Task.Delay( 10 );
        }

        isRunning = false;
        MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = "Cthulhu is hungry for subtitles"; } );
        await Task.Delay( 10 );
    }

    public async Task ProcessVideo( string path )
    {
        
        var ffmpeg = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "unmanaged/ffmpeg.exe" );

        if (!Directory.Exists( System.IO.Path.Combine( System.IO.Path.GetTempPath(), "Cthulhu") ) )
        {
            Directory.CreateDirectory( System.IO.Path.Combine( System.IO.Path.GetTempPath(), "Cthulhu" ) );
        }

        var output = System.IO.Path.Combine( System.IO.Path.GetTempPath(), "Cthulhu", "Fixed" );


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


        if ( !File.Exists( output ) )
        {
            errored = true;
            MainThread.BeginInvokeOnMainThread( () => { lStatus.Text = $"Choked on {path}"; } );
            await Task.Delay( 10 );
            return;
        }

        File.Delete( path );
        File.Copy( output, path );
        File.Delete( output );
    }


}


