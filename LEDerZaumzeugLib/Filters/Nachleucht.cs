using LEDerZaumzeug.Extensions;
using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Macht ein Nachleucht-Effekt. Änderungen des Generators wandern sofort rein,
    /// Alle Pixel werden mit einem Faktor &lt; 1 multipliziert und faden daher aus.
    /// Setze <see cref="Faktor"/> auf 0..1.
    /// </summary>
    public class Nachleucht : IFilter
    {
        private RGBPixel[,] _res;

        /// <summary>Faktor, der draufmultipliziert wird </summary>
        public float Faktor { get; set; } = 0.5f;

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    _res[x, y] = RGBPixel.Max(_res[x,y] * Faktor, pixels[x, y]);
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
