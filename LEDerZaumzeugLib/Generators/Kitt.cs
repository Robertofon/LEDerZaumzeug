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
        private uint sizex, sizey;
        private float richtung = +1;
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
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Speed { get; set; } = 1f;


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
            // Pos
            ulong pos = frame % (ulong)(KittWidth*2);

            // zentrum um herum (gannzzahlig bleiben)
            uint cx = (sizex - (uint)KittWidth) / 2;
            uint cy = (sizey - (uint)1) / 2;
            uint sx = (sizex/2 - (uint)KittWidth/2);
            uint sy = (sizey/2 - (uint)1/2);


            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    if( y >= sy && y < sy+1
                    // je nach richtung x oder y als Laufvariable
                    && x >= sx && x < sx+KittWidth )
                    {
                        pbuf[x,y] = this.Color;
                    }
                    else
                    {
                        pbuf[x, y] = RGBPixel.P0;
                    }
                }
            }

            return Task.FromResult(pbuf);
        }


        private class GObjekt
        {
            public int X;
            public float Y;
            public float Gesch;
        }
    }
}
