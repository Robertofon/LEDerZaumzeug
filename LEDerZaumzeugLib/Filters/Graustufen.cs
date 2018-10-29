using LEDerZaumzeug.Extensions;
using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Gibt ein HSV-Sättigungsmäßig wieder aus.
    /// </summary>
    public class Graustufen : IFilter
    {
        private RGBPixel[,] _res;

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    HSVPixel hsv = pixels[x, y];
                    _res[x, y] = hsv.Grau();
                }
            }

            return Task.FromResult(_res);
        }

        public Task<FilterInfos> GetInfos()
        {
            return Task.FromResult(default(FilterInfos));
        }

        public Task Initialize(MatrixParams matrixParameters)
        {
            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            // Nichts zu tun
        }
    }
}
