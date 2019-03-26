using LEDerZaumzeug.Extensions;
using LEDerZaumzeug.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
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
        private uint sizex, sizey;
        private int pposx, pposy;
        private IEnumerator<Punkt> weg;

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
            Punkt spot = this.weg.Current;
            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if((spot.x == x && spot.y == y && !Invers)
                        || ((spot.x != x || spot.y != y) && Invers))
                    {
                        _res[x, y] = this.Farbe;
                    }
                    else
                    {
                        _res[x, y] = pixels[x, y];
                    }
                }
            }
            this.weg.MoveNext();

            return Task.FromResult(_res);
        }

        public Task<FilterInfos> GetInfos()
        {
            return Task.FromResult(default(FilterInfos));
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            switch (this.Wanderweg)
            {
                case WanderOrder.Zeilenweise:
                    this.weg = GenZeilenweise(this.StartEcke).GetEnumerator();
                    break;
                default:
                    this.weg = GenSpaltenweise(this.StartEcke).GetEnumerator();
                    break;
            }
            pposx = pposy=0;
            return Task.CompletedTask;
        }

        private IEnumerable<Punkt> GenZeilenweise(Ecke start)
        {
            uint xreset = start==Ecke.LinksOben || start==Ecke.LinksUnten ? 0u : this.sizex-1;
            uint xiter = start==Ecke.LinksOben || start==Ecke.LinksUnten ? +1u : -1u;
            uint yreset = start==Ecke.LinksOben || start==Ecke.RechtsOben ? 0u : this.sizey-1;
            uint yiter = start==Ecke.LinksOben || start==Ecke.RechtsOben ? +1u : -1u;
            Punkt p = new Point(xreset,yreset);
            while(true)
            {
                yield return p;

                // weiterzählen
                p = new Punkt(p.x+xiter, p.y);
                if(p.x >= this.sizex || p.x < 0)
                    p = new Punkt(xreset, p.y+yiter);
                if(p.y >= this.sizey || p.y < 0)
                    p = new Punkt(xreset, yreset);
            }
        }

        private IEnumerable<Punkt> GenSpaltenweise(Ecke start)
        {
            uint xreset = start==Ecke.LinksOben || start==Ecke.LinksUnten ? 0 : this.sizex-1;
            uint xiter = start==Ecke.LinksOben || start==Ecke.LinksUnten ? +1u : -1u;
            uint yreset = start==Ecke.LinksOben || start==Ecke.RechtsOben ? 0 : this.sizey-1;
            uint yiter = start==Ecke.LinksOben || start==Ecke.RechtsOben ? +1u : -1u;
            Punkt p = new Point(xreset,yreset);
            while(true)
            {
                yield return p;

                // weiterzählen
                p = new Punkt(p.x, p.y+yiter);
                if(p.y >= this.sizey || p.y < 0)
                    p = new Punkt(p.x+xiter, yreset);
                if(p.x >= this.sizex || p.x < 0)
                    p = new Punkt(xreset, yreset);
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
