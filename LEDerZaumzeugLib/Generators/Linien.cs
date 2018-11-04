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
using SixLabors.Primitives;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der Linien erzeugt.
    /// </summary>
    public class Linien : IGenerator
    {
        private static ILogger _log = NLog.LogManager.GetCurrentClassLogger();
        private int _imgframeCount;
        private uint sizex, sizey;
  
        private RGBPixel[,] pbuf;
        private Image<RgbaVector> _malbild;

        /// <summary>
        /// Geschwindigkeit mit der die Koordinaten weiterwandern.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public LinienTyp Art { get; set; }

        public bool Farbwechsel { get; set;}

        public RGBPixel Farbe { get; set; }

        /// <summary> Dicke der Linien</summary>
        public float N { get; set; } = 1;

        public void Dispose()
        {
            _malbild?.Dispose();
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            var size = new Size((int)this.sizex, (int)this.sizey);
            this.pbuf = new RGBPixel[sizex, sizey];

            _malbild = new Image<RgbaVector>(size.Width, size.Height);

            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            RgbaVector fb = new RgbaVector(this.Farbe.R, Farbe.G, Farbe.B);
            // Male nächste Iteration ins malbild.

            switch (this.Art)
            {
                case LinienTyp.Kreuz:
                break;

                case LinienTyp.Raute:
                break;

                default:
                case LinienTyp.RadarLR:
                case LinienTyp.RadarRL:
                    {
                        ulong koo = (ulong)(frame*Geschwindigkeit) % sizex;
                        koo = (this.Art == LinienTyp.RadarRL)? sizex-koo-1 : koo;
                        _malbild.Mutate( i=>
                        {
                            i.Fill(NamedColors<RgbaVector>.Black);
                            i.DrawLines(new GraphicsOptions(false), fb, this.N, new PointF((float)koo, 0), new PointF((float)koo, sizey));
                        });
                    }
                break;
                case LinienTyp.RadarRLR:
                    {
                        ulong koo = (ulong)(frame*Geschwindigkeit) % (sizex*2);
                        koo = (koo>=sizex)? sizex - (koo % sizex) -1 : koo % sizex;
                        _malbild.Mutate( i=>
                        {
                            i.Fill(NamedColors<RgbaVector>.Black);
                            i.DrawLines(new GraphicsOptions(false), fb, this.N, new PointF((float)koo, 0), new PointF((float)koo, sizey));
                        });
                    }
                break;

                
            }
            // Recycle deinen Puffer und übertrage das Bild dahin.
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    var pxl = _malbild[x, y];
                    pbuf[x, y] = new RGBPixel(pxl.R, pxl.G, pxl.B);
                }
            }
            return Task.FromResult(pbuf);
        }

    }

    public enum LinienTyp
    {
        Kreuz,
        Raute,
        RadarRL,
        RadarRLR,
        RadarLR,
        RadarV
    }
}
