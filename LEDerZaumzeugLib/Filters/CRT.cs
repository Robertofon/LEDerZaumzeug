using LEDerZaumzeug.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Macht auf dem eingabebild diverse ruckler von rechts nach links und links nach rechts.
    /// Jeweils nach dem Zufallsprinzip alle durchschnittlich n frames (zufallsvariation).
    /// </summary>
    public class CRT : IFilter
    {
        private RGBPixel[,] res = null;
        private readonly Random rnd = new Random(Environment.TickCount);
        private ulong _sprung;
        private ulong _schritt;

        /// <summary>
        /// Wie viele Pixel soll die Zeile verschoben sein. Rechts wie links.
        /// </summary>
        public int Weite { get; set; } = 1;

        /// <summary>
        /// Je wieviel Frames soll der Algorithmus einen Ausrutscher machen.
        /// </summary>
        public int Sprung { get; set; } = 30;

        /// <summary>
        /// Max Zahl gleichzeitiger Ausrutscher. Richtung ist zufall.
        /// </summary>
        public int Num { get; set; } = 3;

        public Task<FilterInfos> GetInfos()
        {
            return Task.FromResult(default(FilterInfos));
        }

        public Task Initialize(MatrixParams matrixParameters)
        {
            ErzeugeNextSprung();
            return Task.CompletedTask;
        }

        private void ErzeugeNextSprung()
        {
            // Variiere rund um den Zielwert mit Zufall
            _sprung = (ulong)(this.Sprung + (int)((rnd.NextDouble() - .5) * this.Sprung * 0.6));
            _schritt = 0;
        }

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref res);

            if (_schritt++ == _sprung)
            {
                // füge Ausrutscher hinzu
                var gl = Enumerable.Range(1, Num).Select(_ => (rnd.Next(h), rnd.Next()%2 == 0));
                // erstelle lookup-array mit schiebung je zeile
                int[] shft = new int[h];  // alles 0!! == nix schieben
                foreach(var g in gl)
                {
                    shft[g.Item1] = g.Item2 ? Weite : -Weite;
                }

                // kopie mit schift
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        int nx = shft[y] + x;
                        if (nx < 0 || nx >= w)
                            res[x, y] = RGBPixel.P0;
                        else
                            res[x, y] = pixels[nx, y];
                    }
                }
                ErzeugeNextSprung();
            }
            else
            {
                // normale Kopie
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        res[x, y] = pixels[x, y];
                    }
                }
            }
            return Task.FromResult(res);
        }

        void IDisposable.Dispose()
        {
            // nix zu tun
        }

    }
}
