using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Services;

/// CLIFX: 
/// Can only use public classes as commands, not internal, not private
/// Why Task / Task.CompletedTask instead of await/async ? (async void ExecuteAsync() won't do it?)
/// CommandException -> Shows red error but without stack, other exceptions show the stack
/// ExceptionHandling only works when running the "app.RunAsync()", not when directly executing commands!
/// [Command] --help/-h does show an error, despite working!
/// The --help doesn't show defaults set in Code public string Value { get; set; } = ...
/// Can't show all the arguments on start easy?

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

#if DEBUG
            // Additional Test-Executions before Main execution via args.
            var console = new SystemConsole();

            new Action<IConsole>(async (con) =>
                await new Prototype2ListCommand
                {
                    XmlFilePath = @"D:\Arbeitsmappe\M\iTunes Music Library - Original 14. August 2019 - M.xml",
                    ShowTracks = true,
                    ShowPlaylists = true,
                    MaxPlaylists = 20
                }.ExecuteAsync(con)).Invoke(console);

            new Action<IConsole>(async (con) =>
                await new Prototype2ExportCommand
                {
                    XmlFilePath = "./Resources/Test1.xml",
                    SourceMusicFolderPath = "./Resources/TestSource1/",
                    DestinationMusicFolderRootPath = "./TestDestination1/",
                }.ExecuteAsync(con)).Invoke(console);

            new Action<IConsole>(async (con) =>
                await new Prototype2ExportCommand
                {
                    XmlFilePath = "./Resources/Test2.xml",
                    SourceMusicFolderPath = "./Resources/TestSource2/",
                    DestinationMusicFolderRootPath = "./TestDestination2/",
                }.ExecuteAsync(con)).Invoke(console);
#endif

            //args = new string[] { "p2", "list", "--xml", "D:/Test1.xml" };
            //args = new string[] { "p2 list --help" };
            return await app.RunAsync(args);
        }
    }
}
