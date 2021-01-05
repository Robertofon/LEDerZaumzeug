using System;
using System.ComponentModel;
using System.Threading.Tasks;
using LEDerZaumzeug;

namespace LEDerWand
{
    [Description("Öffnet lokal ein Avalon-Fenster mit LEDs darin in der gewünschten Größe (x*y).")]
    public class AvaWindowOutput : IOutput
    {
        public SizeModes SizeMode { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<OutputInfos> GetInfos()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Initialize(LEDerConfig cfg)
        {
            throw new NotImplementedException();
        }

        public Task Play(RGBPixel[,] pixels)
        {
            throw new NotImplementedException();
        }

        public void SetSize(int rechenDimX, int rechenDimY)
        {
            throw new NotImplementedException();
        }
    }
}
