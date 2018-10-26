using LEDerZaumzeug.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der die Front-LED des Knigt Industries 2000 (K.I.T.T.) nachbaut.
    /// Gehen tut es nur horizontal und Zentriert in Höhe und Breite.
    /// </summary>
    public class Kitt : IGenerator
    {
        private const float KittMinLum = 0.2f;
        private uint sizex, sizey;
        private RGBPixel[,] pbuf;

        private Random rnd = new Random(Environment.TickCount);

        /// <summary>
        /// Parameter to set color.
        /// </summary>
        public RGBPixel Color { get; set; } = RGBPixel.P1;

        /// <summary>
        /// Breite in Pixeln der Frontleuchte.
        /// </summary>
        public int KittWidth { get; set; } = 8;

        /// <summary>
        /// Breite in Pixeln der Frontleuchte.
        /// </summary>
        public int KittHeight { get; set; } = 3;

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Speed { get; set; } = 1f;

        private float[] kittGradient;

        public void Dispose()
        {            
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            kittRes = new float[this.KittWidth];
            kittGradient = new float[this.KittWidth * 2 / 3];
            for (int i = 0; i < kittGradient.Length; i++)
            {
                kittGradient[i] = (1.0f - (i * (0.15f))).LimitTo(KittMinLum, 1f);

            }

            //kittGradient = new float[this.KittWidth * 2 / 3];
            //for(int i=0; i<kittGradient.Length; i++)
            //{
            //    kittGradient[i] = (float)((1d - Math.Log(i) / Math.Log(kittGradient.Length)) / Math.E);
            //    kittGradient[i] = kittGradient[i].LimitTo(KittMinLum, 1.0f);
            //}

            return Task.CompletedTask;
        }

        // kitt zwischenresultat
        float[] kittRes;

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            // Position, wo die animation gerade beginnt
            int kittstartindex = ((int)(frame*Speed)) % (KittWidth*2);
            int bewegungsrichtung = kittstartindex < KittWidth ? -1 : +1;

            // kittRes erstmal auf kleinste lumnianz setzen
            for (int i = 0; i < kittRes.Length; i++)
            {
                kittRes[i] = KittMinLum;
            }

            // Leuchtlauf einblenden mit abblend-gradient dazu.
            try
            {
            int j = kittstartindex;
            for (int i=0; i< kittGradient.Length; i++)
            {
                // Spiegelung erreichen
                int residx = (j >= KittWidth) ? KittWidth - (j % KittWidth) - 1 : j;
                kittRes[residx] = Math.Max(kittGradient[i], kittRes[residx]);
                j-=bewegungsrichtung;
                j = (j + (KittWidth * 2))%(KittWidth*2); // positiv bleiben
            }
            }
            catch(IndexOutOfRangeException e)
            {

            }

            // zentrum um herum (gannzzahlig bleiben)
            uint cx = (sizex - (uint)KittWidth) / 2;
            uint cy = (sizey - (uint)KittHeight) / 2;
            uint sx = (sizex/2 - (uint)KittWidth / 2);
            uint sy = (sizey/2 - (uint)KittHeight / 2);

            for (int x = 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    if (y >= sy && y < sy + KittHeight
                    // je nach richtung x oder y als Laufvariable
                    && x >= sx && x < sx + KittWidth)
                    {
                        pbuf[x, y] = this.Color * kittRes[x - sx];
                    }
                    else
                    {
                        pbuf[x, y] = RGBPixel.P0;
                    }
                }
            }
            return Task.FromResult(pbuf);
        }

    }
}
