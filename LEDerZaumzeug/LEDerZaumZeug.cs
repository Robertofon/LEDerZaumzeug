using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            this.CheckOutputs();
            // Gette erstes Muster aus der Mustersequenz des LED-Programms.
            MusterNode prg1 = this.sequenz.Seq.First();

            var engine = new MusterPipeline(prg1);
            try
            {
                engine.Initialisiere(new MatrixParams() { SizeX = 5, SizeY = 8 });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            RGBPixel[,] bild = await engine.ExecuteAsync(3);
            
        }

        private void CheckOutputs()
        {
            this.outputs.Clear();
            foreach( var outpn in this.config.Outputs)
            {
                // Zeugt die Instanz beim Zugriff
                IOutput o = outpn.Inst;
                this.outputs.Add(o);
            }

            // Erkenne Outputs als die Dimension
            var mo = this.outputs.FirstOrDefault( ou => ou.SizeMode == SizeModes.StaticSetting);
            if( mo != null)
            {
                Console.WriteLine("Fixes Output gefunden: "+ mo.SizeX + "," + mo.SizeY);
            }
        }

        public void Stop()
        {

        }




    }
}
