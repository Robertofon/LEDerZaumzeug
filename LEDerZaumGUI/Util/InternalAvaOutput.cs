using System;
using System.Threading.Tasks;
using Avalonia.Media;
using LEDerWand;
using LEDerZaumzeug;
using LEDerZaumzeug.Outputs;

namespace LEDerZaumGUI.Util
{
    /// <summary>
    /// Implementiert einen Output für ein Avalon control in diesem GUI programm
    /// </summary>
    public class InternalAvaOutput : OutputBase
    {
        public InternalAvaOutput(LedControlViewModel lcvm)
        {
            Lcvm = lcvm;
            SizeX = lcvm.Cols;
            SizeY = lcvm.Rows;
            SizeMode = SizeModes.Static;
            PixelOrder = PixelArrangement.LNH_TL;  // so arbeitet Avalon SimpleGrid
        }

        public LedControlViewModel Lcvm { get; }

        public void Dispose()
        {
        }

        public Task<OutputInfos> GetInfos()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Initialize(LEDerConfig cfg)
        {
            base.Initialize(cfg);
            return Task.FromResult(true);
        }


        public override Task Play(RGBPixel[,] pixels)
        {
            var res = new Color[PxMap.Count];
            for (int io = 0, im = 0; im < PxMap.Count; im++)
            {
                var koord = PxMap[im];
                RGBPixel cpx = pixels[koord.Item1, koord.Item2];
                res[io++] = Color.FromRgb((byte)(cpx.R*255), (byte)(cpx.G*255), (byte)(cpx.B*255));
            }

            this.Lcvm.FeedData(res);
            return Task.CompletedTask;
        }

        public override void SetSize(int rechenDimX, int rechenDimY)
        {
            // nichts machen
        }
    }
}
