using LEDerZaumzeug.Extensions;
using LEDerZaumzeug.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Filter, der ein Bild periodisch durchrollt.
    /// Horizontal oder Vertikal rechts oder links.
    /// Es wird
    /// </summary>
    public class Durchrollen : IFilter
    {
        private uint sizex, sizey;

        private RGBPixel[,] _res;
        private long offset = 0;

        /// <summary>
        /// Ca. 1.0 ist natürlich und gr. kleiner ist faktor für Geschw.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        /// <summary>
        /// Gibt an, in welche Richtung der Inhalt wegscrollt.
        /// </summary>
        public Bewegungrichtung Richtung { get; set; } = Bewegungrichtung.GenOst;

        /// <summary>
        /// Abstand zwischen Anfang und Ende des gerolles
        /// </summary>
        public int Abstand { get; set; } = 1;

        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame)
        {
            if (Richtung == Bewegungrichtung.GenOst || Richtung == Bewegungrichtung.GenWest)
            {
                offset = (offset + 1) % sizex;
            }
            else
            {
                offset = (offset + 1) % sizey;
            }

            (int w, int h) = pixels.EnsureArray2D(ref _res);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    switch (Richtung)
                    {
                        case Bewegungrichtung.GenOst:
                        {
                            long xx = (x + offset) % (Abstand + sizex);
                            _res[x, y] = xx >= sizex ? RGBPixel.P0 : pixels[xx, y];
                        }
                            break;
                        case Bewegungrichtung.GenWest:
                        {
                            long xx = (x + offset) % (Abstand + sizex);
                            _res[x, y] = xx >= sizex ? RGBPixel.P0 : pixels[xx, y];
                        }
                            break;
                        case Bewegungrichtung.GenNord:
                        {
                            long yy = (y + offset) % (Abstand + sizey);
                            _res[x, y] = yy >= sizey ? RGBPixel.P0 : pixels[x, yy];
                        }
                            break;
                        default:
                        {
                            long yy = (y - offset) % (Abstand + sizey);
                            _res[x, y] = yy >= sizey ? RGBPixel.P0 : pixels[x, yy];
                        }
                            break;

                    }
                }
            }

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
            return Task.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            // nichts zu Disposen
        }

    }

}
