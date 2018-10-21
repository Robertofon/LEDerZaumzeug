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
    /// Generator, der eine Sinuswelle vertikaler oder horizontaler ausbreitung in einer Farbe
    /// darstellt. Mit Hintergrundfarbe überblendet.
    /// </summary>
    public class Wave : IGenerator
    {
        private uint sizex, sizey;
        private RGBPixel[,] pbuf;
        
        private Random rnd = new Random(Environment.TickCount);

        /// <summary>
        /// Parameter to set color.
        /// </summary>
        public RGBPixel Color { get; set; } = RGBPixel.P1;

        public RGBPixel BgColor { get; set; } = RGBPixel.P0;

        /// <summary>
        /// Wellenlänge in Pixel. Also die Frequenz dessen.
        /// </summary>
        public float WaveLenght { get; set; } = 8;

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Speed { get; set; } = 1f;

        public Richtung Direction { get; set; } = Richtung.O;

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
            // Rechne die Phasenverschiebung
            double phase = frame / WaveLenght * (Math.PI*2) * this.Speed;
            // invertiere sie bei Süd und West!
            if( this.Direction == Richtung.W || this.Direction == Richtung.S )
            {
                phase = -phase;
            }

            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    // je nach richtung x oder y als Laufvariable
                    int w = (this.Direction == Richtung.W || this.Direction == Richtung.O) ? x : y;
                    float val = .5f+ (float)(Math.Sin(phase + (w / WaveLenght * (2 * Math.PI))) * 0.5);

                    // Anwenden von linearem additiven überblenden zwischen Forder- und Hintergrund
                    pbuf[x, y] = (this.Color * val) + (this.BgColor * (1-val));
                }
            }

            return Task.FromResult(pbuf);
        }
    }
}
