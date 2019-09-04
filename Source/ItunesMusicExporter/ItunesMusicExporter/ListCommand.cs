using System;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Services;
using Newtonsoft.Json;
using static ItunesMusicExporter.DirtyUtils;

namespace ItunesMusicExporter
{
    [Command("list", Description = "Lists or exports the tracks and playlists inside an iTunes music library XML file 'iTunes Music Library.xml'.")]
    public class ListCommand : ICommand
    {
        [CommandOption("input-path", IsRequired = true, Description = "Mandatory. String. Path to the iTunes music library xml file. Can be relative or absolute.")]
        public string MusicLibraryFilePathOption { get; set; }

        [CommandOption("tracks", 't', IsRequired = false, Description = "Flag. Set if you want to see a listing of the tracks.")]
        public bool ListTracksOption { get; set; } = false;

        [CommandOption("max-tracks", IsRequired = false, Description = "Number. Option for 'tracks'. Defines how many tracks are listed. Default = 10.")]
        public int MaxTracksOption { get; set; } = 10;

        [CommandOption("playlists", 'p', IsRequired = false, Description = "Flag. Set if you want to see a listing of the playlists")]
        public bool ListPlaylistsOption { get; set; } = false;

        [CommandOption("max-playlists", IsRequired = false, Description = "Number. Option for 'playlists'. Defines how many playlists are listed. Default = 10.")]
        public int MaxPlaylistsOption { get; set; } = 10;

        [CommandOption("output-to-file", 'o', IsRequired = false, Description = "Flag. Set if you want to output the result to a file instead of the console window.")]
        public bool OutputToFileOption { get; set; } = false;

        [CommandOption("output-format", IsRequired = false, Description = "String. Option for 'output-to-file'. What format is used for the output. Available: 'json', 'xml', 'plaintext'. Default = 'plaintext'.")]
        public string OutputFormatOption { get; set; } = "plaintext";

        [CommandOption("output-path", IsRequired = false, Description = "String. Option for 'output-to-file'. Where the file is (over-)written. Can be relative or absolute. Default = './Output.data'.")]
        public string OutputFilePathOption { get; set; } = "./Output.data";

        public Task ExecuteAsync(IConsole console)
        { 
            // Abort if the file is non existent.
            if (!System.IO.File.Exists(MusicLibraryFilePathOption))
                throw new ArgumentException("Specified path to XML file invalid. Please check 'xml'", nameof(MusicLibraryFilePathOption));

            var tracks = GetTracks(MusicLibraryFilePathOption);
            var playlists = GetTracksAndPlaylistsCombined(MusicLibraryFilePathOption);

            string output = "";
            string crlf = Environment.NewLine;

            string format = OutputFormatOption;

            // Check for invalid format
            if (!new string[] {"plaintext", "json", "xml" }.Contains(format))
            {
                console.Output.WriteLine($"Format '{format}' unknown. Defaulting to 'plaintext'.");
                format = "plaintext";
            }

            if (format == "plaintext")
            {
                if (ListTracksOption)
                {
                    output += $"> Tracks:" + crlf;
                    tracks.Take(MaxTracksOption).ToList().ForEach(track => output += $"> > {track.Key}: {track.Name}" + crlf);
                }

                if (ListPlaylistsOption)
                {
                    output += $"> Playlists:" + crlf;
                    playlists.Take(MaxPlaylistsOption).ToList().ForEach(pl => output += $"> > {pl.Id} ({pl.Tracks.Count()}): {pl.Name}" + crlf);
                }
            }
            else if (format == "json")
            {
                output += "{\"Tracks\":";
                if (ListTracksOption)
                {
                    output += JsonConvert.SerializeObject(tracks, Formatting.Indented);
                }
                else
                {
                    output += "[]";
                }
                output += "," + crlf;

                output += "\"Playlists\":";
                if (ListPlaylistsOption)
                {
                    output += JsonConvert.SerializeObject(playlists, Formatting.Indented);
                }
                else
                {
                    output += "[]";
                }
                output += "}";
            }
            else if (format == "xml")
            {
                output += "{\"Tracks\":";
                if (ListTracksOption)
                {
                    output += JsonConvert.SerializeObject(tracks);
                }
                else
                {
                    output += "[]";
                }
                output += "," + crlf;

                output += "\"Playlists\":";
                if (ListPlaylistsOption)
                {
                    output += JsonConvert.SerializeObject(playlists);
                }
                else
                {
                    output += "[]";
                }
                output += "}";

                output = JsonConvert.DeserializeXmlNode(output, "root").OuterXml;
            }

            if (OutputToFileOption)
            {
                string path = OutputFilePathOption;
                System.IO.File.WriteAllText(path, output);
                console.WithForegroundColor(ConsoleColor.Green, () => console.Output.WriteLine($"Result successfully exported in '{format}' to [{System.IO.Path.GetFullPath(path)}]."));
            }
            else 
                console.WithForegroundColor(ConsoleColor.Green, () => console.Output.Write(output));
            
            return Task.CompletedTask;
        }
    }


}