using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, animierte GIF anzeigt.
    /// </summary>
    public class GIF : IGenerator
    {
        private static ILogger _log = NLog.LogManager.GetCurrentClassLogger();
        private int _imgframeCount;
        private Bitmap[] _images = new Bitmap[0];
        private uint sizex, sizey;
  
        private FrameDimension _dimension;
        private RGBPixel[,] pbuf;
        
        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public float Winkel { get; set; } = 0f;

        public string Pfad { get; set; } = string.Empty;


        public void Dispose()
        {  
            foreach (var item in _images)
            {
                item.Dispose();
            }
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            if(!string.IsNullOrEmpty(Pfad))
            {
                using(FileStream pngStream = new FileStream(Pfad,FileMode.Open, FileAccess.Read))
                {
                    using(var image = new Bitmap(pngStream))
                    {
                        _dimension = new FrameDimension(image.FrameDimensionsList[0]);
                        _imgframeCount = image.GetFrameCount(_dimension);
                        _images = new Bitmap[_imgframeCount];
                        for(int k = 0; k<_imgframeCount; k++)
                        {
                            image.SelectActiveFrame(_dimension, k);
                            var destRect = new Rectangle(0, 0, (int)sizex, (int)sizey);
                            var destImage = new Bitmap(destRect.Width, destRect.Height);

                            using (var graphics = Graphics.FromImage(destImage))
                            {
                                graphics.CompositingMode = CompositingMode.SourceCopy;
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                using (var wrapMode = new ImageAttributes())
                                {
                                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                                }
                            }
                            _images[k] = destImage;
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            
            // Winkel berechnen in rad
            double wr = this.Winkel / 180d * Math.PI;

            int k = (int)((frame/Geschwindigkeit) % (float)_imgframeCount);
            _log.Debug("k="+k + " dim: " + _dimension.ToString());
            //Image image = Image.GetInstance(_image, System.Drawing.Imaging.ImageFormat.Bmp);
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    Color pxl = _images[k].GetPixel(x, y);
                    pbuf[x, y] = new RGBPixel(pxl.R/255f, pxl.G/255f, pxl.B/255f);
                }
            }
            return Task.FromResult(pbuf);
        }
    }
}
