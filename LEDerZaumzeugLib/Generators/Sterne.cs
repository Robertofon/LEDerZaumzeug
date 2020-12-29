using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der Sterne der Farbe <see cref="Color"/> aufblitzen lässt.
    /// </summary>
    [Description("Aufblitzen von Sternen")]
    public class Sterne : IGenerator
    {
        private uint sizex, sizey;
        private Random rnd = new Random(Environment.TickCount);
        private RGBPixel[,] pbuf;
        private List<Stern> _st;
        /// <summary>
        /// Farbe der Sterne.
        /// </summary>
        public RGBPixel Color { get; set; } = RGBPixel.P1;

        public float Geschwindigkeit { get; set; } = 1;

        public int Anzahl { get; set; } = 30;

        public void Dispose()
        {            
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];
            _st = new List<Stern>();
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            
            Sternenmanagement();
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    pbuf[x, y] = RGBPixel.P0;
                }
            }

            foreach( var st in _st)
            {
                pbuf[st.X, st.Y] = this.Color * st.Hell;
            }

            return Task.FromResult(pbuf);
        }

        void Sternenmanagement()
        {
            // ausglimmen lassen
            _st = _st.Select( s => new Stern(){X=s.X, Y=s.Y, Hell=0.8f* s.Hell }).ToList();

            // tote sterne weg:
            _st = _st.Where( s=>s.Hell > 0.01f).ToList();

            // auftoppen
            int diff = this.Anzahl - _st.Count();
            int topup = Convert.ToInt32(diff/4.0);
            for(int i=0; i<topup; i++)
            {
                Stern stern = new Stern()
                { 
                    X=(rnd.Next((int)sizex)),
                    Y=(rnd.Next((int)sizey)),
                    Hell=1.0f 
                };
                if(stern.X>=sizex || stern.Y>=sizey)
                    Debugger.Break();
                _st.Add(stern);
            }
        }

        private class Stern
        {
            public int X, Y;
            public float Hell;
        }
    }
}
