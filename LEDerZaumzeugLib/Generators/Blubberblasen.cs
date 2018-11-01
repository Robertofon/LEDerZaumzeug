﻿using System;
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
using SixLabors.Shapes;


namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der Bilder oder auch animierte GIF/PNG anzeigt.
    /// </summary>
    public class Blubberblasen : IGenerator
    {
        private static ILogger _log = NLog.LogManager.GetCurrentClassLogger();
        private Image<RgbaVector> _image;
        private uint sizex, sizey;
        private Random _rnd = new Random(Environment.TickCount);
        private int _maxBlasen;
        private int _maxR;
        private RGBPixel[,] pbuf;
        private List<Blase> _bl = new List<Blase>();

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = .51f;


        public void Dispose()
        {
            _image?.Dispose();
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            var size = new Size((int)this.sizex, (int)this.sizey);
            this.pbuf = new RGBPixel[sizex, sizey];
            this._image = new Image<RgbaVector>(size.Width, size.Height);

            // Blasenanzahl etwa 10% der pixelzahl ?!?!
            this._maxBlasen = (size.Height * size.Width) / 10;
            this._maxR = Math.Min(size.Height, size.Width) / 8;
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            UpdateBlasen(frame);

            // Recycle deinen Puffer
            // Male den nächsten Rahmen in das Bild.
            _image.Mutate(ctx =>
            {
                //ctx.
                ctx.Fill(RgbaVector.Black);
                foreach (Blase b in _bl)
                {
                    var e = new EllipsePolygon(b.X, b.Y, b.R);
                    ctx.Fill(b.Fb, e);
                }
            });
            
            // Bild übertragen...
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    RgbaVector pxl = _image[x, y];
                    pbuf[x, y] = new RGBPixel(pxl.R, pxl.G, pxl.B);
                }
            }
            return Task.FromResult(pbuf);
        }

        private void UpdateBlasen(ulong frame)
        {
            // sterben lassen .. alles was es bis oben geschafft hat und aus dem bild ist.
            _bl = _bl.Where(bl => bl.Y >= -_maxR).ToList();

            // topup (+1), wenn zu wenige  ( Rate bei vertikale Auflösung* geschwindigkeit)
            if(_bl.Count() < _maxBlasen  && (frame % (1+(ulong)((sizey/Geschwindigkeit)/ _maxBlasen)) == 0)) 
            {
                _bl.Add(new Blase()
                {
                    X = (float)(_rnd.NextDouble() * sizex),
                    Y = sizey + 1,
                    R = (float)(_rnd.NextDouble() * _maxR),
                    Fb = new RgbaVector((float)_rnd.NextDouble(), (float)_rnd.NextDouble(), (float)_rnd.NextDouble())
                });
            }

            // wanderung für alle
            for( int i=0; i<_bl.Count; i++)
            {
                // Muss so raus und rein, weil struct!!
                var bl = _bl[i];
                bl.X += (float)((_rnd.NextDouble()- .5) * this.Geschwindigkeit) ;
                bl.Y -= (float)(_rnd.NextDouble() * this.Geschwindigkeit);
                _bl[i] = bl;
            }
        }

        private struct Blase
        {
            public float X, Y, R;
            public RgbaVector Fb;
        }
    }
}
