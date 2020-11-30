using System;
using System.Collections.Generic;
using System.ComponentModel;
using SixLabors.ImageSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der den Diamond-Square-Algorithmen für Landschaften für
    /// Bunte Farbverläufe nutzt und anomation dazwischen erzeugt.
    /// </summary>
    public class Plasma : IGenerator
    {
        private uint sizex, sizey;
        private Random rnd = new Random(Environment.TickCount);
  
        private RGBPixel[,] pbuf;
        private float[,] _startbild, _zielbild;

        /// <summary>
        /// Geschwindigkeit mit der die Koordinaten weiterwandern.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        /// <summary> Breite und Höhe des Generierten Bildes.</summary>
        public int N { get; set; } = 1;

        public void Dispose()
        {
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            _startbild = DiamondBrick(N);
            _zielbild = DiamondBrick(N);

            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Male nächste Iteration ins malbild.
            ulong schritt = frame % 100;
            float f = schritt/100f;

            // Bild mit modulo wiederholen (immer Quadratisch)
            int d = _startbild.GetLength(0);
            // Recycle deinen Puffer und übertrage das Bild dahin.
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    var pxl = (_zielbild[x%d,y%d] * (f)) + _startbild[x%d, y%d] * (1.0f-f);
                    pbuf[x, y] = new HSVPixel(pxl, 1, 1);
                }
            }

            // Wechsle Bilder durch alle x frames
            if(schritt == 0)
            {
                _startbild = _zielbild;
                _zielbild  = DiamondBrick(N);
            }

            return Task.FromResult(pbuf);
        }

        private float[,] DiamondBrick(int n)
        {
            int dim = (1<<n) +1;
            var landschaft = new float[dim,dim];
            // Die 4 Ecken sind initialisiert == ergo mit 0.0f!!
            Quadrat(landschaft, 0, 0, dim-1, dim-1);
            // normieren  0..360
            float max = landschaft.Aggregate(float.MinValue, Math.Max);
            float min = landschaft.Aggregate(float.MaxValue, Math.Min);
            landschaft.Each( g => g *360/(max-min) );

            return landschaft;
        }

        private void Quadrat(float[,] land, int stx, int sty, int edx, int edy)
        {
            // Abbruchbed
            if( edx-stx <= 1 || edy-sty <= 1 )
            {
                return;
            }

            // die vier Ecken a, b, 
            //                c, d ermitteln.
            float a = land[stx, sty];
            float b = land[edx, sty];
            float c = land[stx, edy];
            float d = land[edx, edy];
            // Mittelpunkt der vier Randpunkte berechnen (Rechteckfase)
            int mpx = (stx+edx)/2;
            int mpy = (sty+edy)/2;
            land[mpx, mpy] = (a + b + c + d)/4 + (float)rnd.NextDouble();

            // Diamantstufe machen  
            //(erst rechts davon, dann drunter)
            Diamant(N, land, mpx, sty, mpx + (edx-stx), edy );
            Diamant(N, land, stx, mpy, edx, mpy + (edy-sty));
            //(links davon und drunter) 
            Diamant(N, land, mpx-(edx-stx), sty, mpx, edy );
            Diamant(N, land, stx, mpy-(edy-sty), edx, mpy );

            // Jetzt 4fach-Rekursion
            Quadrat(land, stx, sty, mpx, mpy);
            Quadrat(land, mpx, mpy, edx, edy);
            Quadrat(land, mpx, sty, mpx, edy);
            Quadrat(land, stx, mpy, edx, mpy);
        }

        private void Diamant(int n, float[,] land, int stx, int sty, int edx, int edy)
        {
            // Diamantphase: die vier Diamantecken ermitteln. Mitte ausmitteln
            // Dabei kann über Arraygrenzen gegangen werden. Daher mit Array-dimensin MOD
            int d = (1<<n);

            int mpx = (stx+edx)/2;
            int mpy = (sty+edy)/2;
            stx += d;  // mod von negativen Zahlen vermeiden
            sty += d;
            land[mpx, mpy] = (land[stx%d, mpy] + land[mpx, sty%d] 
                            + land[edx%d, mpy] + land[mpx, edy%d])
                                / 4 + (float)rnd.NextDouble();
        }

    }

}
