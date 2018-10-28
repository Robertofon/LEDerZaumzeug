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
    /// Generator, der graustufiges Rauschen erzeugt.
    /// </summary>
    public class Rauschen : IGenerator
    {
        private uint sizex, sizey;
        private Random rnd = new Random(Environment.TickCount);
  
        private RGBPixel[,] pbuf;
    
        /// <summary>
        /// Farbe und somit maximale auslenkung der Rauschinfo. </summary>
        public RGBPixel Farbe { get; set; } = RGBPixel.P1;

        public void Dispose()
        {            
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            // Winkel berechnen in rad
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    double d = (rnd.NextDouble());
                    pbuf[x, y] = Farbe * (float)d;
                }
            }

            return Task.FromResult(pbuf);
        }
    }
}
