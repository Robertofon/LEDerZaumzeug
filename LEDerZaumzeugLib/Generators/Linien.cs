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
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

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

        /// <summary>
        /// Art des bildes: Kreuz, Raute, RadarLR, RadarRL, RadarRLR, RadarVO, RadarOV, RadarVOV.
        /// </summary>
        public LinienTyp Art { get; set; }

        /// <summary>
        /// <c>true</c> für Farbwechsel im Farbkreis. false für konsant <see cref="Farbe"/>.
        /// </summary>
        public bool Farbwechsel { get; set;}

        /// <summary>
        /// Farbe, wenn <see cref="Farbwechsel"/> nicht aktiv ist.
        /// </summary>
        public RGBPixel Farbe { get; set; }

        /// <summary> Dicke der Linien</summary>
        public float N { get; set; } = 1;

        /// <summary> Anzahl der Linien, wenn anwendbar.</summary>
        public int Anzahl { get; set; } = 4; 

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
            Color fb = this.Farbe;
            // Male nächste Iteration ins malbild.

            switch (this.Art)
            {
                case LinienTyp.Kreuz:
                    {
                        ulong w = (ulong)(frame*Geschwindigkeit) % 100;
                        double wr = w /100d * Math.PI;
                        double ph = 2*Math.PI / Anzahl;
                        PointF mp = new PointF(sizex/2f, sizey/2f);
                        _malbild.Mutate( (IImageProcessingContext i)=>
                        {
                            i.Fill(Color.Black);
                            for(int l = 0; l<this.Anzahl; l++)
                            {
                                i.DrawLines(new ShapeGraphicsOptions() { GraphicsOptions = { Antialias = false } }, fb, this.N, 
                                    mp, new PointF(mp.X + (float)Math.Cos(wr+ph*l)*sizex, mp.Y + (float)Math.Sin(wr+ph*l)*sizey));
                            }
                        });
                    }
                break;

                case LinienTyp.Raute:
                    {
                        float w = 0.01f * ((ulong)(frame*Geschwindigkeit) % 100);
                        _malbild.Mutate( i=>
                        {
                            i.Clear(Color.Black);
                            i.DrawLines(new ShapeGraphicsOptions(){GraphicsOptions = {Antialias = false}}, fb, this.N, 
                                new PointF(sizex*w, 0),
                                new PointF(sizex, sizey*w),
                                new PointF(sizex*(1-w), sizey),
                                new PointF(0, sizey*(1-w)),
                                new PointF(sizex*w, 0));
                        });
                    }
                break;

                default:
                case LinienTyp.RadarLR:
                case LinienTyp.RadarRL:
                    {
                        ulong koo = (ulong)(frame*Geschwindigkeit) % sizex;
                        koo = (this.Art == LinienTyp.RadarRL)? sizex-koo-1 : koo;
                        _malbild.Mutate( i=>
                        {
                            i.Clear(Color.Black);
                            i.DrawLines(new ShapeGraphicsOptions() { GraphicsOptions = { Antialias = false } }, 
                                fb, this.N, new PointF((float)koo, 0), new PointF((float)koo, sizey-1));
                        });
                    }
                break;
                case LinienTyp.RadarRLR:
                    {
                        ulong koo = (ulong)(frame*Geschwindigkeit) % (sizex*2);
                        koo = (koo>=sizex)? sizex - (koo % sizex) -1 : koo % sizex;
                        ILineSegment s = new LinearLineSegment(new PointF((float)koo, 0), new PointF((float)koo, sizey - 1));
                        IPath p= new SixLabors.ImageSharp.Drawing.Path(s);
                        _malbild.Mutate( i=>
                        {
                            i.Clear(Color.Black);
                            i.Draw(new ShapeGraphicsOptions() { GraphicsOptions = { Antialias = false } },
                                fb, N, p);
                        });
                    }
                break;
                case LinienTyp.RadarVO:
                case LinienTyp.RadarOV:
                case LinienTyp.RadarVOV:
                    throw new NotImplementedException();
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

            if(this.Farbwechsel)
            {
                HSVPixel h = this.Farbe;
                h.H += 1;
                this.Farbe = h;
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
        RadarVO,
        RadarOV,
        RadarVOV
    }
}
