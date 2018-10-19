using LEDerZaumzeug.Extensions;
using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Gibt ein Bild 1:1 wieder aus, jedoch alle Werte auf 0,0..1,0 zurechtgestutzt.
    /// </summary>
    public class Clip01 : IFilter
    {
        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.Dim();
            var res = new RGBPixel[w,h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    res[x, y] = pixels[x, y].Clip();
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
    }
}
