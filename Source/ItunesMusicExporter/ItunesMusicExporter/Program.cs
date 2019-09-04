using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliFx;
using CliFx.Services;

/// CLIFX: 
/// Can only use public classes as commands, not internal, not private
/// Why Task / Task.CompletedTask instead of await/async ? (async void ExecuteAsync() won't do it?)
/// CommandException -> Shows red error but without stack, other exceptions show the stack
/// ExceptionHandling only works when running the "app.RunAsync()", not when directly executing commands!
/// The --help doesn't show defaults set in Code public string Value { get; set; } = ...
/// Can't show all the arguments on start easy?
/// No Examples??

namespace ItunesMusicExporter
{
    /// <summary>
    /// Prototype 2 for the ItunesMusicExporter -> Export/Copy Music (MP3-Files) from iTunes-Storage-Folder into playlist-folders.
    /// More quick than dirty, advanced features, no hard coding.
    /// Status: TODO
    /// See: https://github.com/Countryen/itunes-music-exporter/projects/2
    /// September 2019, Countryen @ C0 | .NET Core v2.2
    /// </summary>
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // https://github.com/Tyrrrz/CliFx
            ICliApplication app = new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                //.UseTitle("")
                //.UseVersionText("")
                //.UseExecutableName("")
                //.UseDescription("")
                .AllowDebugMode(true)
                .AllowPreviewMode(true)
                .Build();

            await RunTestsDebugAsync(info: false, list: true, export: false, diff: false);

            //args = new string[] { "p2", "list", "--xml", "D:/Test1.xml" };
            //args = new string[] { "p2 list --help" };
            //return await app.RunAsync(args);
            return 0;
        }

        static async Task RunTestsDebugAsync(bool info, bool list, bool export, bool diff)
        {
#if DEBUG
            // Additional Test-Executions before Main execution via args.
            var console = new SystemConsole();

            var RunTestCommandAsync = new Func<string, IConsole, ICommand, Task>(async (log, con, com) =>
            {
                con.WithForegroundColor(ConsoleColor.DarkBlue, () => con.Output.WriteLine(log));
                await com.ExecuteAsync(con);
            });

            

            var RunTestCommand = new Action<string, IConsole, ICommand>((log, con, com) =>
            {
                RunTestCommandAsync(log, con, com).Wait();
            });

            #region TEST-INFO
            if (info)
            {
                await RunTestCommandAsync("info --input-path './Resources/Test1.xml'", 
                    console,
                    new InfoCommand
                    {
                        MusicLibraryFilePathOption = "./Resources/Test1.xml",
                    });

                await RunTestCommandAsync("info --input-path './Resources/Test2.xml'", 
                    console,
                    new InfoCommand
                    {
                        MusicLibraryFilePathOption = "./Resources/Test2.xml",
                    });

                await RunTestCommandAsync("info --input-path 'D:\\Arbeitsmappe\\M\\iTunes Music Library - Original 14. August 2019 - M.xml'",
                    console,
                    new InfoCommand
                    {
                        MusicLibraryFilePathOption = "D:\\Arbeitsmappe\\M\\iTunes Music Library - Original 14. August 2019 - M.xml",
                    });
            }
            #endregion
            #region #region TEST-LIST
            if (list)
            {

                await RunTestCommandAsync("list -t --input-path 'M.xml'",
                    console,
                    new ListCommand
                    {
                        MusicLibraryFilePathOption = @"D:\Arbeitsmappe\M\iTunes Music Library - Original 14. August 2019 - M.xml",
                        ListTracksOption = true,
                    });

                await RunTestCommandAsync("list -tp --input-path 'M.xml' --max-tracks 20 --max-playlists 20",
                    console,
                    new ListCommand
                    {
                        MusicLibraryFilePathOption = @"D:\Arbeitsmappe\M\iTunes Music Library - Original 14. August 2019 - M.xml",
                        ListTracksOption = true,
                        ListPlaylistsOption = true,
                        MaxTracksOption = 20,
                        MaxPlaylistsOption = 20,
                    });

                await RunTestCommandAsync("list -tp --input-path 'M.xml' --max-tracks 20 --max-playlists 20 -o --output-format 'xml' --output-file 'M.xml'",
                    console,
                    new ListCommand
                    {
                        MusicLibraryFilePathOption = @"D:\Arbeitsmappe\M\iTunes Music Library - Original 14. August 2019 - M.xml",
                        ListTracksOption = true,
                        ListPlaylistsOption = true,
                        MaxTracksOption = 50,
                        MaxPlaylistsOption = 50,
                        OutputToFileOption = true,
                        OutputFormatOption = "xml",
                        OutputFilePathOption = "M.xml",
                    });

                await RunTestCommandAsync("list -tp --input-path 'M.xml' --max-tracks 20 --max-playlists 20 -o --output-format 'json' --output-file 'M.json'",
                    console,
                    new ListCommand
                    {
                        MusicLibraryFilePathOption = @"D:\Arbeitsmappe\M\iTunes Music Library - Original 14. August 2019 - M.xml",
                        ListTracksOption = true,
                        ListPlaylistsOption = true,
                        MaxTracksOption = 50,
                        MaxPlaylistsOption = 50,
                        OutputToFileOption = true,
                        OutputFormatOption = "json",
                        OutputFilePathOption = "M.json",
                    });
            }
            #endregion
            #region TEST-EXPORT
            if (export)
            {
                await RunTestCommandAsync("export --input-path './Resources/Test2.xml' --src './Resources/TestSource2/' --dest './TestDestination2/'",
                    console,
                    new ExportCommand
                    {
                        MusicLibraryFilePathOption = "./Resources/Test2.xml",
                        SourceMusicFolderPathOption = "./Resources/TestSource2/",
                        DestinationMusicFolderRootPathOption = "./TestDestination2/",
                    });
            }
            #endregion
            #region TEST-DIFF
            if (diff)
            {

            }
            #endregion
#endif
        }
    }
}
