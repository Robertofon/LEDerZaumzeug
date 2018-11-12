using System;
using System.Collections.Generic;
using System.ComponentModel;
using SixLabors.ImageSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, versucht eine Feueranimation zu erzeugen.
    /// </summary>
    public class Feuer : IGenerator
    {
        private uint sizex, sizey;
        private Random rnd = new Random(Environment.TickCount);
  
        private RGBPixel[,] pbuf;

        /// <summary>
        /// Geschwindigkeit mit der die Koordinaten weiterwandern.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public void Dispose()
        {
        }

        private float _f;

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            // Faktor bestimmen. F soll sizey-3 mal multipliziert werden, ehe 1 zu 0,05 wird
            if( sizey <4 )
            {   
                _f=.5f;
            }
            {
                _f = (float)Math.Pow(0.05, 1.0/(sizey-3) );
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Algorithmus verwendet keine Partikeltechnik.
        /// Stattdessen wird einfach nur in der untersten Y-Zeile mit random
        /// verschiedene Rot+Gelbtöne gezündelt. Diese werden dann von oben her
        /// nach oben vererbt und ausgeblendet mit f*f, f&lt;1.
        /// </summary>
        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Vererbe alle Zeilen von oben kommend eins hoch mal Ausblendfaktor
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey-1; y++)
                {
                    pbuf[x, y] = pbuf[x,y+1] * _f;
                }
            }

            // Jetzt unterste Zeile neu befeuern:
            for( int x=0; x< sizex; x++)
            {
                // nehme die farben rot (0°) bis gelb (60°) und intensitäten 0.3-1
                pbuf[x,sizey-1] = new HSVPixel((float)rnd.NextDouble()*60f,1,(float)rnd.NextDouble()*.3f+.7f);
            }

            return Task.FromResult(pbuf);
        }

    }

}
