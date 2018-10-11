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
        private IEnumerable<IOutput> outputs;

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

        public void Stop()
        {

        }




    }
}
