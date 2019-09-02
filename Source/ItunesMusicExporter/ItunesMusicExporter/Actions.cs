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

namespace ItunesMusicExporter
{
    static class Actions
    {
        public static void ExtractPlaylistsSim(string path, string pathSrc, string pathDest)
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

        public static dynamic DeserializeXmlDynamic(string path)
        {
            XDocument doc = XDocument.Load(path); //or XDocument.Parse(text)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic result = JsonConvert.DeserializeObject<dynamic>(jsonText);
            return result;
        }

        public static dynamic DeserializeXmlExpando(string path)
        {
            // Expando object is a bit of a hassle because of this: https://stackoverflow.com/questions/26778554/why-cant-i-index-into-an-expandoobject
            XDocument doc = XDocument.Load(path); //or XDocument.Parse(text)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
            return result;
        }

        public static string GetMusicFolder(string path)
        {

            var o = DeserializeXmlDynamic(path);
            string result = o.plist.dict["string"][2];

            return result;
        }

        public static IList<(int Key, string Name, string Location)> GetTracks(string path)
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
                    string location = track["string"].Last;

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

        public static IList<(int Id, string Name, int[] Tracks)> GetPlaylists(string path)
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
                JToken idContainer = pl["integer"];
                if (idContainer.HasValues)
                    id = idContainer[0].Value<int>();
                else
                    id = idContainer.Value<int>();

                string name = pl["string"][1].Value<string>();

                // If PL contains only 1 Track, it's an JObject not an JArray. JObjects are indexed like Dictionaries.
                tracks.Clear();
                if (pl.ToObject<JObject>().ContainsKey("array"))
                {
                    JToken tracksContainer = pl["array"]["dict"];
                    if (tracksContainer.Type == JTokenType.Array)
                    {
                        foreach (var track in tracksContainer)
                            tracks.Add(track["integer"].Value<int>());
                    }
                    else
                        tracks.Add(tracksContainer["integer"].Value<int>());
                }

                result.Add((id, name, tracks.ToArray()));
            }

            return result;
        }

        public static IList<(string Name, IEnumerable<(int Id, string Name, string Location)> Tracks)> GetTracksAndPlaylistsCombined(string path)
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
