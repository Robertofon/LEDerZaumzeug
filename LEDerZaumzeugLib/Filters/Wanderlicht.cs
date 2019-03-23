using LEDerZaumzeug.Extensions;
using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Filter, der ein Bild mit einem Pixel schritweise durchlässt oder
    /// auf eine Farbe setzt. So kann mit einem statischen Hintergrund
    /// ein wandernder Pixel das Bild durchlassen oder auf einen Wert setzen. 
    /// </summary>
    public class Wanderlicht : IFilter
    {
        private RGBPixel[,] _res;

        /// <summary>
        /// Macht die Erscheinung invers.
        /// </summary>
        /// <value><c>true</c> Wanderpixel hat in <see cref="Farbe"/> angegebene Farbe,
        /// der Rest die Farbe des drunterliegenden Generators.
        /// <c>false</c>: Wanderpixel hat die Farbe des Generatrs, der Rest
        /// die angegebene Farbe.</value>
        public bool Invers { get; set; }

        /// <summary>
        /// Farbe in RGB, die entweder der Wanderpixel oder der Rest haben soll.
        /// Empfehlung: Schwarz. Siehe Invers.
        /// </summary>
        public RGBPixel Farbe { get; set; } = RGBPixel.P1;

        /// <summary>
        /// Ca. 1.0 ist natürlich und gr. kleiner ist faktor für Geschw.
        /// </summary>
        public float Geschwindigkeitsfaktor { get; set; } = 1f;

        /// <summary>
        /// Spezifiziert, welchen Modus der Wanderweg des Pixels beschreiten sollte.
        /// </summary>
        public WanderOrder Wanderweg { get; set; }

        /// <summary>
        /// Startecke, in dem die Wanderung beginnen soll.
        /// </summary>
        /// <value></value>
        public Ecke StartEcke { get; set; } = Ecke.LinksOben;

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    _res[x, y] = pixels[x, y];
                }
            }

            return Task.FromResult(_res);
        }

        private int pposx, pposy;

        public Task<FilterInfos> GetInfos()
        {
            return Task.FromResult(default(FilterInfos));
        }

        public Task Initialize(MatrixParams matrixParameters)
        {
            switch (this.Wanderweg)
            {
                case WanderOrder.Zeilenweise:
                    return GenZeilenweise(this.StartEcke);
                default:
            }
            pposx = pposy=0;
            return Task.CompletedTask;
        }

        private IEnumerable<Punkt> GenZeilenweise(Ecke start)
        {
            Punkt p = Startpunkt(start);
            while(true)
            {
                yield p;
                p = new Punkt(p.x+1, p.y);
                if( p.x>)

            }
        }

        void IDisposable.Dispose()
        {
            // nichts zu Disposen
        }

    }

    public enum WanderOrder
    {
        SchneckeUzgA2I,
        SchneckeUzgI2A,
        SchneckeGUzgA2I,
        SchneckeGUzgI2A,
        Zeilenweise,
        Spaltenweise,
        Zeilenmeander,
        Spaltenmeander,
    }
}
