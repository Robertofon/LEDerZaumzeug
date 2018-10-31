using LEDerZaumzeug.Extensions;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Basisklasse für Faltungen mit einem 3x3-Kern
    /// </summary>
    public abstract class FaltungBasis : IFilter
    {
        private RGBPixel[,] _res;
        protected float[] _kern;
        protected float _sVoll;
        protected float _sEol, _sEor, _sEul, _sEur;
        protected float _sKl, _sKr, _sKo, _sKu;
        
        public virtual Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    _res[x, y] = Kern(pixels, x, y, w, h);
                }
            }

            return Task.FromResult(_res);
        }

        /// <summary>
        /// Wendet einen Filterkern (_kern) an und verlässt sich auf vorher
        /// ausgerechnete summen der Ecken und Kanten. Symmetrie ist nicht erforderlich
        /// </summary>
        /// <param name="pixels">Pixelbild der Quelle - zu faltend</param>
        /// <param name="x">Faltung an Stelle x.</param>
        /// <param name="y">Faltung an Stelle y.</param>
        /// <param name="mx">Max X, muss mit pixels.GetDimension(0) passen.</param>
        /// <param name="my">Max X, muss mit pixels.GetDimension(1) passen.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RGBPixel Kern(RGBPixel[,] pixels, int x, int y, int mx, int my)
        {
            if (x < 1) // x linker rand
            {
                if (y < 1) // ohne oben,links  == mit unten rechts
                    return (pixels[x, y]  *_kern[4] + pixels[x+1, y]  *_kern[5] 
                          + pixels[x, y+1]*_kern[7] + pixels[x+1, y+1]*_kern[8]) / _sEur;
                else if (y+1 >= my) // ohne unten,links = mit oben rechts
                    return (pixels[x, y - 1] * _kern[1] + pixels[x + 1, y - 1] * _kern[2]
                          + pixels[x, y] * _kern[4] + pixels[x + 1, y] * _kern[5]) / _sEor;
                else // x rand, y gut = ohne links == mit rechts
                    return (pixels[x, y - 1] * _kern[1] + pixels[x + 1, y - 1] * _kern[2]
                          + pixels[x, y]     * _kern[4] + pixels[x + 1, y]     * _kern[5]
                          + pixels[x, y + 1] * _kern[7] + pixels[x + 1, y + 1] * _kern[8]) / _sKr;
            }
            else if( x+1 >= mx) // x rechter rand
            {
                if (y < 1) // x,y rand = ohne oben,rechts = mit untenlinks
                    return (pixels[x-1, y]  *_kern[3] + pixels[x, y]    *_kern[4]  
                          + pixels[x-1, y+1]*_kern[6]+ pixels[x, y+1]  *_kern[7] ) / _sEul;
                else if (y+1 >= my) // x,y rand = ohne unten,rechts = mit obenlinks
                    return (pixels[x - 1, y - 1] * _kern[0] + pixels[x, y - 1] * _kern[1]
                          + pixels[x - 1, y] * _kern[3] + pixels[x, y]     * _kern[4] ) / _sEol;
                else // x rand, y gut = ohne rechts
                    return (pixels[x - 1, y - 1] * _kern[0] + pixels[x, y - 1] * _kern[1] 
                          + pixels[x - 1, y]     * _kern[3] + pixels[x, y]     * _kern[4] 
                          + pixels[x - 1, y + 1] * _kern[6] + pixels[x, y + 1] * _kern[7] ) / _sKl;
            }
            else // x mittendrin
            {
                if (y < 1) // x ok, y rand = ohne oben = mit unten
                    return (pixels[x - 1, y]     * _kern[3] + pixels[x, y]     * _kern[4] + pixels[x + 1, y]     * _kern[5] 
                          + pixels[x - 1, y + 1] * _kern[6] + pixels[x, y + 1] * _kern[7] + pixels[x + 1, y + 1] * _kern[8] ) / _sKu;
                else if (y+1 >= my) // x ok,y rand = ohne unten  = mit oben
                    return ( pixels[x - 1, y - 1] * _kern[0] + pixels[x, y - 1] * _kern[1] + pixels[x + 1, y - 1] * _kern[2]
                           + pixels[x - 1, y]     * _kern[3] + pixels[x, y]     * _kern[4] + pixels[x + 1, y]     * _kern[5]) / _sKo;
                else // alles inmitten - nimm alle 9
                    return ( pixels[x - 1, y - 1] * _kern[0] + pixels[x, y - 1] * _kern[1] + pixels[x + 1, y - 1] * _kern[2]
                           + pixels[x - 1, y]     * _kern[3] + pixels[x, y]     * _kern[4] + pixels[x + 1, y]     * _kern[5]
                           + pixels[x - 1, y + 1] * _kern[6] + pixels[x, y + 1] * _kern[7] + pixels[x + 1, y + 1] * _kern[8]) / _sVoll;
            }
        }

        public virtual Task<FilterInfos> GetInfos()
        {
            return Task.FromResult(default(FilterInfos));
        }

        protected abstract float[] InitKernel();

        public virtual Task Initialize(MatrixParams matrixParameters)
        {
            _kern = InitKernel();
            if(_kern.Length != 9)
            {
                throw new ArgumentException("Feld von InitKernel muss float[9] sein!");
            }

            // 0  1  2
            // 3  4  5  
            // 6  7  8
            _sEol = _kern[0] + _kern[1] + _kern[3] + _kern[4];
            _sEor = _kern[1] + _kern[2] + _kern[4] + _kern[5];
            _sEul = _kern[3] + _kern[4] + _kern[6] + _kern[7];
            _sEur = _kern[4] + _kern[5] + _kern[7] + _kern[8];
            _sKo = _kern[0] + _kern[1] + _kern[2] + _kern[3] + _kern[4] + _kern[5];
            _sKu = _kern[3] + _kern[4] + _kern[5] + _kern[6] + _kern[7] + _kern[8];
            _sKl = _kern[0] + _kern[1] + _kern[3] + _kern[4] + _kern[6] + _kern[7];
            _sKr = _kern[1] + _kern[2] + _kern[4] + _kern[5] + _kern[7] + _kern[8];
            _sVoll = _kern.Sum();
            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            // nichts zu tun
        }
    }
}
