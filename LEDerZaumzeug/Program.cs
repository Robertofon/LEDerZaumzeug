using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{

    class Program
    {
        private const string StdPath = "Mini.ledp";
        //private const string StdPath = "Sequenz.ledp";
        private const string CfgPath = "config.json";

        static async Task Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLog.LogManager.GetCurrentClassLogger();
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
                    await pixelator.StartAsync();
                    await pixelator.Run();
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


    }
}
