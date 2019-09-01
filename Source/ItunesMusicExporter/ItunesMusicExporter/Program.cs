using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using System.Dynamic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ItunesMusicExporter
{
    /// <summary>
    /// Prototype 1 for the ItunesMusicExporter -> Export/Copy Music (MP3-Files) from iTunes-Storage-Folder into playlist-folders.
    /// Quick and dirty, trying things out, using highly instable code and without any best practices used.
    /// September 2019, Countryen @ C0 | .NET Core v2.2
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            new Action(() =>
            {
                string PATH1 = "./Resources/Test1.xml";
                string PATH2 = "./Resources/TestSource1/";
                string PATH3 = "./TestDestination1/";

                Console.WriteLine("Start");
                ExpandoObject expandoXml = DeserializeXmlExpando(PATH1);
                dynamic dynamicXml = DeserializeXmlDynamic(PATH1);

                string musicFolder = GetMusicFolder(PATH1);
                var tracks = GetTracks(PATH1);
                var playlists = GetPlaylists(PATH1);
                var together = GetTracksAndPlaylistsCombined(PATH1);
                ExtractPlaylistsSim(PATH1, PATH2, PATH3);
            }).Invoke();

            new Action(() =>
            {
                string PATH1 = "./Resources/Test2.xml";
                string PATH2 = "./Resources/TestSource2/";
                string PATH3 = "./TestDestination2/";

                Console.WriteLine("Start");
                ExpandoObject expandoXml = DeserializeXmlExpando(PATH1);
                dynamic dynamicXml = DeserializeXmlDynamic(PATH1);

                string musicFolder = GetMusicFolder(PATH1);
                var tracks = GetTracks(PATH1);
                var playlists = GetPlaylists(PATH1);
                var together = GetTracksAndPlaylistsCombined(PATH1);
                ExtractPlaylistsSim(PATH1, PATH2, PATH3);
            }).Invoke();

        }

        private static void ExtractPlaylistsSim(string path, string pathSrc, string pathDest)
        {
            var pls = GetTracksAndPlaylistsCombined(path);

            foreach (var pl in pls)
            {
                foreach (var track in pl.Tracks)
                {
                    string origPath = track.Location;
                    string simSrcPath = origPath.Replace("file:///Users/testuser/Music/iTunes/iTunes%20Media/Music/The%20Best%20Artist/Unknown%20Album/", pathSrc);
                    string simDestPath = System.IO.Path.Combine(pathDest, pl.Name);
                    System.IO.Directory.CreateDirectory(simDestPath);
                    System.IO.File.Copy(simSrcPath, System.IO.Path.Combine(simDestPath, track.Name + ".mp3"), true);
                }
            }
        }

        static dynamic DeserializeXmlDynamic(string path)
        {
            XDocument doc = XDocument.Load(path); //or XDocument.Parse(text)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic result = JsonConvert.DeserializeObject<dynamic>(jsonText);
            return result;
        }

        static dynamic DeserializeXmlExpando(string path)
        {
            // Expando object is a bit of a hassle because of this: https://stackoverflow.com/questions/26778554/why-cant-i-index-into-an-expandoobject
            XDocument doc = XDocument.Load(path); //or XDocument.Parse(text)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
            return result;
        }

        static string GetMusicFolder(string path)
        {
            
            var o = DeserializeXmlDynamic(path);
            string result = o.plist.dict["string"][2];

            return result;
        }

        static IList<(int Key, string Name, string Location)> GetTracks(string path)
        {
            var result = new List<(int Key, string Name, string Location)>();
            var o = DeserializeXmlDynamic(path);

            // If only 1 Track exists, it's an JObject not an JArray. JObjects are indexed like Dictionaries.
            var tracksKeys = o.plist.dict.dict.key;
            var tracks = o.plist.dict.dict.dict;
            if (tracks.Type == JTokenType.Array)
            {
                var len = tracksKeys.Count;

                for (int i = 0; i < len; i++)
                {
                    var track = tracks[i];

                    int key = tracksKeys[i];
                    string title = track["string"][2];
                    string location = track["string"][5];

                    result.Add((key, title, location));
                }
            }
            else
            {
                var track = tracks;

                int key = tracksKeys;
                string title = track["string"][2];
                string location = track["string"][5];

                result.Add((key, title, location));
            }
            

            return result;
        }

        static IList<(int Id, string Name, int[] Tracks)> GetPlaylists(string path)
        {
            var result = new List<(int Id, string Name, int[] Tracks)>();
            dynamic o = DeserializeXmlDynamic(path);

            // If only 1 PL exists, it's an JObject not an JArray. JObjects are indexed like Dictionaries.
            JArray playlists;
            JToken playlistsContainer = o.plist.dict.array.dict;
            if (playlistsContainer.Type == JTokenType.Array)
            {
                playlists = playlistsContainer as JArray;
            }
            else
                playlists = new JArray(playlistsContainer);

            int len = playlists.Count;

            var tracks = new List<int>();
            int id;

            for (int i = 0; i < len; i++)
            {
                JToken pl = playlists[i];

                // If only 1 value with name "integer" exists, it's no JArray. (e.g. the Master "Mediathek")
                id = 0;
                JToken idContainer = pl["integer"];
                if (idContainer.HasValues)
                    id = idContainer[0].Value<int>();
                else
                    id = idContainer.Value<int>();

                string name = pl["string"][1].Value<string>();

                // If PL contains only 1 Track, it's an JObject not an JArray. JObjects are indexed like Dictionaries.
                tracks.Clear();
                JToken tracksContainer = pl["array"]["dict"];
                if (tracksContainer.Type == JTokenType.Array)
                {
                    foreach (var track in tracksContainer)
                        tracks.Add(track["integer"].Value<int>());
                }
                else
                    tracks.Add(tracksContainer["integer"].Value<int>());

                result.Add((id, name, tracks.ToArray()));
            }

            return result;
        }

        static IList<(string Name, IEnumerable<(int Id, string Name, string Location)> Tracks)> GetTracksAndPlaylistsCombined(string path)
        {
            var tracks = GetTracks(path);
            var playlists = GetPlaylists(path);

            return playlists.Select(x =>
            {
                (string Name, IEnumerable<(int Id, string Name, string Location)> Tracks) y;
                y.Name = x.Name;
                y.Tracks = x.Tracks.Select(t => (t, tracks.First(i => i.Key == t).Name, tracks.First(i => i.Key == t).Location)).ToArray();

                return y;
            }).ToList();
        }
            
    }
}
