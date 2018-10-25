using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{

    class Program
    {
        private const string StdPath = "Mini.ledp";
        //private const string StdPath = "Sequenz.ledp";
        private const string CfgPath = "config.json";
        // NLog: setup the logger first to catch all errors
        private static ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            var path = @"./PlugIns";
            var repo = new PluginTypeRepo();
            repo.WithAssembliesInPath(path);
            var mix =repo.GetTypesImplementing<IMixer>();
            var flt =repo.GetTypesImplementing<IFilter>();
            var outp =repo.GetTypesImplementing<IOutput>();

            try
            {
                NLog.LogManager.ThrowExceptions = true;
                logger.Debug("init main");
                LEDerConfig lederconfig = null;
                Console.WriteLine("LEDerZaumzeug!\nPixelgenerator meiner Wahl.");
                Console.WriteLine("lese Konfig: " + CfgPath);
                using (var stream = File.OpenRead(CfgPath))
                {
                    lederconfig = await SerialisierungsFabrik.ReadConfigFromStreamAsync(stream);
                }

                PixelProgram programmsequenz = null;
                using (Stream stream = File.OpenRead(StdPath))
                {
                    programmsequenz = await SerialisierungsFabrik.ReadProgramFromStreamAsync(stream);
                }

                using (var pixelator = new LEDerZaumZeug(lederconfig, programmsequenz))
                {
                    await pixelator.AddOutputsFromCfg();
                    await pixelator.StartAsync();

                    Console.WriteLine("ENTER zum Beenden!!!!!");
                    Console.ReadLine();

                    // Synce auf
                    pixelator.Stop();
                    Console.WriteLine("==========");
                } 
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        private static bool pred(Type obj)
        {
            return true;
        }
    }

    public class PluginTypeRepo
    {
        private List<System.Reflection.Assembly> assemblies = new List<System.Reflection.Assembly>();
        public PluginTypeRepo()
        {

        }


        public void WithAssembliesInPath(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            //var asi = AssemblyLoadContext.Default.LoadFromAssemblyPath(path + "\\PluginsFSharp.dll")
            //                ;
            var assemblies = Directory

                .GetFiles(path, "*.dll", searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
                //.ToList();
            this.assemblies.AddRange(assemblies);
        }

        public IEnumerable<Type> GetTypesImplementing<T>()
        {
            return assemblies.SelectMany(
                ass=>ass
                .GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(T))))
                ;
            
        }
    }
}
