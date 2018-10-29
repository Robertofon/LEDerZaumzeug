using LEDerZaumzeug.Extensions;
using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Wendet einen Faktor auf das Eingabebild an. Also Pixel*
    /// <see cref="Skalar"/>.
    /// </summary>
    public class Faktor : IFilter
    {
        private RGBPixel[,] _res;

        public float Skalar { get; set; } = 1;

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    _res[x, y] = pixels[x, y] * this.Skalar;
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
            // nichts zu Disposen
        }

    }
}
