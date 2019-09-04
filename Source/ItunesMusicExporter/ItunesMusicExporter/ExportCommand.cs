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
using static ItunesMusicExporter.DirtyUtils;

namespace ItunesMusicExporter
{
    [Command("export", Description = "Exports all given entries in the XML File from a source folder to a destionation folder.")]
    public class ExportCommand : ICommand
    {
        [CommandOption("input-path", IsRequired = true, Description = "Mandatory. String. Path to the iTunes music library xml file. Can be relative or absolute.")]
        public string MusicLibraryFilePathOption { get; set; }

        [CommandOption("src", IsRequired = true, Description = "Path to the music folder, in which iTunes stores the music. Can be relative or absolute.")]
        public string SourceMusicFolderPathOption { get; set; }

        [CommandOption("dest", IsRequired = true, Description = "Path to the desired export folder, in which playlist-sub-folder will be created. Can be relative or absolute.")]
        public string DestinationMusicFolderRootPathOption { get; set; }

        public Task ExecuteAsync(IConsole console)
        {
            ExtractPlaylistsSim(MusicLibraryFilePathOption, SourceMusicFolderPathOption, DestinationMusicFolderRootPathOption);

            return Task.CompletedTask;
        }
    }


}