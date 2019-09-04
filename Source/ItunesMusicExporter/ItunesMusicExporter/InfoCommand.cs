using System;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Services;
using static ItunesMusicExporter.DirtyUtils;

namespace ItunesMusicExporter
{
    [Command("info", Description = "Returns information about an iTunes music library XML file 'iTunes Music Library.xml'.")]
    public class InfoCommand : ICommand
    {
        [CommandOption("input-path", IsRequired = true, Description = "Mandatory. String. Path to the iTunes music library xml file. Can be relative or absolute.")]
        public string MusicLibraryFilePathOption { get; set; }

        public Task ExecuteAsync(IConsole console)
        {
            // Abort if the file is non existent.
            if (!System.IO.File.Exists(MusicLibraryFilePathOption))
                throw new ArgumentException("Specified path to XML file invalid. Please check 'xml'", nameof(MusicLibraryFilePathOption));

            string musicFolder = GetMusicFolder(MusicLibraryFilePathOption);
            var tracks = GetTracks(MusicLibraryFilePathOption);
            var playlists = GetPlaylists(MusicLibraryFilePathOption);

            console.WithForegroundColor(ConsoleColor.Green, () =>
            {
                console.Output.WriteLine($"Information:");
                console.Output.WriteLine($"> Path: {MusicLibraryFilePathOption}");
                console.Output.WriteLine($"> Lines: {System.IO.File.ReadAllLines(MusicLibraryFilePathOption).Length}");
                console.Output.WriteLine($"> Music Folder: {musicFolder}");
                console.Output.WriteLine($"> Number of Tracks: {tracks.Count}");
                console.Output.WriteLine($"> Number of Playlists: {playlists.Count}");
            });

            return Task.CompletedTask;
        }
    }
}