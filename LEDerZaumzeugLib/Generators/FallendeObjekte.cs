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
    /// Generator für zufällig fallende objekte der Länge 1..3
    /// </summary>
    public class FallendeObjekte : IGenerator
    {
        private uint sizex, sizey;
        private RGBPixel[,] pbuf;
        private LinkedList<GObjekt> objekte = new LinkedList<GObjekt>();
        private Random rnd = new Random(Environment.TickCount);

        /// <summary>
        /// Parameter to set color.
        /// </summary>
        public RGBPixel Color { get; set; } = RGBPixel.P1;

        public int ObjektZahl { get; set; } = 8;

        /// <summary>
        /// Ca. 1.0 ist natürlich und gr. kleiner ist faktor für Geschw.
        /// </summary>
        public float Geschwindigkeitsfaktor { get; set; } = 1f;


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
            if( frame %8 ==0)
            {
                this.ObjekteAufstocken();
            }


            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    // male Objekt, wo es ist
                    if( this.objekte.Any( o=>o.X == x && Convert.ToInt32(o.Y) == y))
                    {
                        pbuf[x, y] = this.Color;
                    }
                    else
                    {
                        pbuf[x, y] = RGBPixel.P0;
                    }
                }
            }

            this.ObjekteWanderung();

            return Task.FromResult(pbuf);
        }

        private void ObjekteWanderung()
        {
            foreach( GObjekt g in this.objekte)
            {
                g.Y += g.Gesch * this.Geschwindigkeitsfaktor;
            }

            foreach (var toteobjekte in this.objekte.Where(o => o.Y > this.sizey).ToList())
            {
                this.objekte.Remove(toteobjekte);
            }
        }

        private void ObjekteAufstocken()
        {
            // 1..3 Objekte neu jedes Mal
            int male = rnd.Next(2) + 1;
            for (int mal = 0; mal < male ; mal++ )
            {
                // Zielzahl unterschritten. 1+
                if (this.objekte.Count < this.ObjektZahl)
                {
                    this.objekte.AddLast(new GObjekt()
                    {
                        X = rnd.Next((int)this.sizex),
                        Y = 0,
                        Gesch = (float)rnd.NextDouble() + .5f
                    });
                }

            }
        }

        private class GObjekt
        {
            public int X;
            public float Y;
            public float Gesch;
        }
    }
}
