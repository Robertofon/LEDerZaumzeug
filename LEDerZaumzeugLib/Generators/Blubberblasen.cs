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
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
  
        private RGBPixel[,] pbuf;
        
        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;


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
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            // Male den nächsten Rahmen in das Bild.
            _image.Mutate(ctx =>
            {
                //ctx.
                ctx.Fill(RgbaVector.Black);
                var e = new EllipsePolygon(new PointF(3.5f, 5), 3);
                ctx.Fill(RgbaVector.Red, e);
            }
            );
            
            ulong k = ((ulong)(frame*Geschwindigkeit));
            //Image image = Image.GetInstance(_image, System.Drawing.Imaging.ImageFormat.Bmp);
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
    }
}
