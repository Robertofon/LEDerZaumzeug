using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Hauptklasse, instanzieren um zu arbeiten.
    /// </summary>
    public class LEDerZaumZeug : IDisposable
    {
        private readonly LEDerConfig config;
        private PixelProgram sequenz;
        private MusterPipeline activePipeline;
        private readonly List<IOutput> outputs = new List<IOutput>();
        // Rechengrößen für max dimensionen.
        private uint masterSizeX, masterSizeY;

        public LEDerZaumZeug(LEDerConfig config, PixelProgram sequenz)
        {
            this.config = config;
            this.sequenz = sequenz;
            this.activePipeline = null;
        }

        public void Dispose()
        {
            Stop();
        }

        public async Task StartAsync()
        {
            Console.Write("Start");

            // Outputs bearbeiten
            await this.InitOutputsAsync();
            // Gette erstes Muster aus der Mustersequenz des LED-Programms.
            MusterNode prg1 = this.sequenz.Seq.First();

            var engine = new MusterPipeline(prg1);
            try
            {
                engine.Initialisiere(new MatrixParams() { SizeX = masterSizeX, SizeY = masterSizeY });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            this.activePipeline = engine;
        }

        public async Task Run()
        {
            var sw = new Stopwatch();
            sw.Start();
            // Bilder in der Schleife generieren
            for (int i = 0; i < 100; i++)
            {
                RGBPixel[,] bild = await this.activePipeline.ExecuteAsync(i);
                var outputtasks = this.outputs.Select( output => output.Play(bild));
                await Task.WhenAll(outputtasks.ToArray());
            }
            sw.Stop();
            Console.WriteLine("100 bilder dauerten: " + TimeSpan.FromTicks(sw.ElapsedTicks));
        }



        private async Task InitOutputsAsync()
        {
            this.outputs.Clear();
            foreach( var outpn in this.config.Outputs)
            {
                // Zeugt die Instanz beim Zugriff
                IOutput o = outpn.Inst;
                bool erfolg = await o.Initialize(this.config);
                if (erfolg == true)
                {
                    this.outputs.Add(o);
                }
                else
                {
                    Console.WriteLine("lasse Output " + o.GetType().Name + " weg.");
                }
            }

            // Erkenne Outputs als die Dimension
            var masterout = this.outputs.FirstOrDefault( ou => ou.SizeMode == SizeModes.Static);
            if( masterout != null)
            {
                Console.WriteLine("Fixes Output gefunden: "+ masterout.SizeX + "," + masterout.SizeY);
                this.masterSizeX = (uint)masterout.SizeX;
                this.masterSizeY = (uint)masterout.SizeY;
                foreach( var outpn in this.outputs)
                {
                    outpn.SetSize(masterout.SizeX, masterout.SizeY);
                }
            }


        }

        public void Stop()
        {

        }




    }
}
