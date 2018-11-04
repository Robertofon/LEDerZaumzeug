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
    /// Generator, der Bilder oder auch animierte GIF/PNG anzeigt.
    /// </summary>
    public class Bild : IGenerator
    {
        private static ILogger _log = NLog.LogManager.GetCurrentClassLogger();
        private int _imgframeCount;
        private uint sizex, sizey;
  
        private RGBPixel[,] pbuf;
        private Image<Rgba32> _resized;

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public float Winkel { get; set; } = 0f;

        public string Pfad { get; set; } = string.Empty;


        public void Dispose()
        {
            _resized?.Dispose();
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            var size = new Size((int)this.sizex, (int)this.sizey);
            this.pbuf = new RGBPixel[sizex, sizey];

            if(!string.IsNullOrEmpty(Pfad))
            {
                using(FileStream pngStream = new FileStream(Pfad,FileMode.Open, FileAccess.Read))
                {
                    using(var image = SixLabors.ImageSharp.Image.Load(pngStream))
                    {
                        _resized = image.Clone(
                        ctx => ctx.Resize(
                            new ResizeOptions
                            {
                                Size = size,
                                Mode = ResizeMode.Crop,
                                Sampler = new BoxResampler()
                            }));
                        _imgframeCount = _resized.Frames.Count.LimitTo(1, int.MaxValue);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            int k = (int)((ulong)(frame*Geschwindigkeit) % (ulong)_imgframeCount);
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    Rgba32 pxl = _resized.Frames[k][x, y];
                    pbuf[x, y] = new RGBPixel(pxl.R/255f, pxl.G/255f, pxl.B/255f);
                }
            }
            return Task.FromResult(pbuf);
        }
    }
}
