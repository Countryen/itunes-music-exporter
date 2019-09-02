using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using System.Dynamic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using CliFx;
using System.Threading.Tasks;
using CliFx.Services;
using CliFx.Attributes;

/// CLIFX: 
/// Can only use public classes as commands, not internal, not private
/// hy Task / Task.CompletedTask instead of await/async ? (async void ExecuteAsync() won't do it?)

namespace ItunesMusicExporter
{
    /// <summary>
    /// Prototype 2 for the ItunesMusicExporter -> Export/Copy Music (MP3-Files) from iTunes-Storage-Folder into playlist-folders.
    /// More quick than dirty, advanced features, no hard coding.
    /// Status: TODO
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


            // Sim, will be removed later.
            if (args.Length == 0)
                args = new[] { "p12" };

            return await app.RunAsync(args);
        }
    }
}

