using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Klasse ist ein Output und verschluckt alles.
    /// Indes: Man kann die Größe der Matrix setzen.
    /// </summary>
    public class NullOutput : IOutput
    {
        public SizeModes SizeMode { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public void Dispose()
        {
        }

        public Task<OutputInfos> GetInfos()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Initialize(LEDerConfig config)
        {
            return Task.FromResult(true);
        }

        public Task Play(RGBPixel[,] pixels)
        {
            Console.WriteLine($"Sende Bild pixels[{pixels.GetLength(0)},{pixels.GetLength(1)}]");
            return Task.CompletedTask;
        }

        public void SetSize(int rechenDimX, int rechenDimY)
        {
            SizeX = rechenDimX; SizeY = rechenDimY;
        }
    }
}
