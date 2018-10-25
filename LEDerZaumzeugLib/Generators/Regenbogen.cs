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
    /// Generator, der einen HSV-Regenbogen macht.
    /// </summary>
    public class Regenbogen : IGenerator
    {
        private uint sizex, sizey;
  
        private RGBPixel[,] pbuf;
        
        /// <summary>
        /// Wellenlänge in Pixel. Also die Frequenz dessen.
        /// </summary>
        public float Lambda { get; set; } = 18;

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public float Winkel { get; set; } = 45f;

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
            double wr = this.Winkel / 180d * Math.PI;
            double sinwr = Math.Sin(wr);
            double coswr = Math.Cos(wr);
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    float fb = (frame*Geschwindigkeit) +
                        ((float)(y * sinwr+ x * coswr)  * 360f / Lambda);
                    pbuf[x, y] = new HSVPixel(fb, .9f, .9f);
                }
            }

            return Task.FromResult(pbuf);
        }
    }
}
