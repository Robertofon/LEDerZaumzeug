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

        public Task StartAsync()
        {
            MusterNode prg = this.sequenz.Seq.First();
            Console.Write("Start");
            return Task.Delay(800);
            
        }

        public void Stop()
        {

        }




    }
}
