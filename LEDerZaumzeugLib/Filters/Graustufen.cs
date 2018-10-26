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
        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.Dim();
            var res = new RGBPixel[w,h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    HSVPixel hsv = pixels[x, y];
                    res[x, y] = hsv.Grau();
                }
            }

            return Task.FromResult(res);
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
        }
    }
}
