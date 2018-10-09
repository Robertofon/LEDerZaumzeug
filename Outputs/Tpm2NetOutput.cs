using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Output implementierung für TPM2.Net protokoll.
    /// Siehe: http://www.tpm2.de/
    /// </summary>
    public class Tpm2NetOutput : IOutput
    {
        private List<(int, int)> pxmap = new List<(int, int)>();


        public void Dispose()
        {
            
        }

        public Task<OutputInfos> GetInfos()
        {
            return Task.FromResult(new OutputInfos());
        }

        public Task Initialize(object paramset)
        {
            // Pixelorder herausbekommen, Mapping machen
            GenerischeOrder(pxmap, PixelArrangement.SNH_TR);
            
            
            // UDP socket aufmachen
            return Task.Delay(0);


        }

        private void GenerischeOrder(List<(int, int)> pxmap, PixelArrangement sNH_TR)
        {
            throw new NotImplementedException();
        }

        public async Task Play(RGBPixel[,] pixels)
        {
            // Den Scheiß an den Mann bringen
            await Task.Delay(3);
        }
    }
}
