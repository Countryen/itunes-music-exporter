using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CliFx;
using CliFx.Attributes;
using CliFx.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static ItunesMusicExporter.Actions;

namespace ItunesMusicExporter
{
    [Command("p2 export", Description = "Prototype 2 - Export mode. Exports all given entries in the XML File from a source folder to a destionation folder. ")]
    public class Prototype2ExportCommand : ICommand
    {
        [CommandOption("xml", IsRequired = true, Description = "Path to the input iTunes music library xml file. Can be relative or absolute.")]
        public string XmlFilePath { get; set; }

        [CommandOption("src", IsRequired = true, Description = "Path to the music folder, in which iTunes stores the music. Can be relative or absolute.")]
        public string SourceMusicFolderPath { get; set; }

        [CommandOption("dest", IsRequired = true, Description = "Path to the desired export folder, in which playlist-sub-folder will be created. Can be relative or absolute.")]
        public string DestinationMusicFolderRootPath { get; set; }

        public Task ExecuteAsync(IConsole console)
        {
            string thisCommandName = this.GetType().CustomAttributes.First(attrib => attrib.AttributeType.Name == "CommandAttribute").ConstructorArguments[0].ToString();
            console.Output.WriteLine($"Starting Command {thisCommandName} with {nameof(XmlFilePath)}={XmlFilePath} and {nameof(SourceMusicFolderPath)}={SourceMusicFolderPath} and {nameof(DestinationMusicFolderRootPath)}={DestinationMusicFolderRootPath}");

            ExtractPlaylistsSim(XmlFilePath, SourceMusicFolderPath, DestinationMusicFolderRootPath);

            console.Output.WriteLine($"Ending Command {thisCommandName}");

            return Task.CompletedTask;
        }
    }

    [Command("p2 list", Description = "Prototype 2 - List Mode. Lists information about the entries in the XML File.")]
    public class Prototype2ListCommand : ICommand
    {
        [CommandOption("xml", IsRequired = true, Description = "Mandatory. Path to the input iTunes music library xml file. Can be relative or absolute.")]
        public string XmlFilePath { get; set; }

        [CommandOption("tracks", 't', IsRequired = false, Description = "Flag. Set if you want to see a listing of the tracks inside the XML. Default = False")]
        public bool ShowTracks { get; set; } = false;

        [CommandOption("tracksMax", IsRequired = false, Description = "Number. Combined with 'tracks' you can define, how many tracks you want to list. Default = 10")]
        public int MaxTracks { get; set; } = 10;

        [CommandOption("tracksSkipped", IsRequired = false, Description = "Number. Combined with 'tracks' you can define, how many tracks you want to skip. Default = 0")]
        public int SkippedTracks { get; set; } = 0;

        [CommandOption("playlists", 'p', IsRequired = false, Description = "Flag. Set if you want to see a listing of the playlists inside the XML. Default = False")]
        public bool ShowPlaylists { get; set; } = false;

        [CommandOption("playlistsMax", IsRequired = false, Description = "Number. Combined with 'playlists' you can define, how many playlists you want to list. Default = 10")]
        public int MaxPlaylists { get; set; } = 10;

        [CommandOption("playlistsSkipped", IsRequired = false, Description = "Number. Combined with 'playlists' you can define, how many playlists you want to skip. Default = 0")]
        public int SkippedPlaylists { get; set; } = 0;


        public Task ExecuteAsync(IConsole console)
        {
            string thisCommandName = this.GetType().CustomAttributes.First(attrib => attrib.AttributeType.Name == "CommandAttribute").ConstructorArguments[0].ToString();
            console.Output.WriteLine($"Starting Command {thisCommandName} with {nameof(XmlFilePath)}={XmlFilePath}");

            // Abort if the file is non existent.
            if (!System.IO.File.Exists(XmlFilePath))
                throw new ArgumentException("Specified path to XML file invalid. Please check 'xml'", nameof(XmlFilePath));

            string musicFolder = GetMusicFolder(XmlFilePath);
            var tracks = GetTracks(XmlFilePath);
            var playlists = GetPlaylists(XmlFilePath);

            console.WithForegroundColor(ConsoleColor.Green, () =>
            {
                console.Output.WriteLine($"Information about XML File:");
                console.Output.WriteLine($"> Path: {XmlFilePath}");
                console.Output.WriteLine($"> Lines: {System.IO.File.ReadAllLines(XmlFilePath).Length}");
                console.Output.WriteLine($"> Music Folder: {musicFolder}");
                console.Output.WriteLine($"> Number of Tracks: {tracks.Count}");
                console.Output.WriteLine($"> Number of Playlists: {playlists.Count}");
                console.Output.WriteLine();
                
                if (ShowTracks)
                {
                    console.Output.WriteLine($"> Tracks:");
                    tracks.Skip(SkippedTracks).Take(MaxTracks).ToList().ForEach(track => console.Output.WriteLine($"> > {track.Key}: {track.Name}"));
                }

                
                if (ShowPlaylists)
                {
                    console.Output.WriteLine($"> Playlists:");
                    playlists.Skip(SkippedPlaylists).Take(MaxPlaylists).ToList().ForEach(playlist => console.Output.WriteLine($"> > {playlist.Id} ({playlist.Tracks.Length}): {playlist.Name}"));
                }
            });

            console.Output.WriteLine($"Ending Command {thisCommandName}");

            return Task.CompletedTask;
        }
    }


}