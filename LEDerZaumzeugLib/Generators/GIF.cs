using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, animierte GIF anzeigt.
    /// </summary>
    public class GIF : IGenerator
    {
        private int _gifframeCount;
        private Image _image;
        private uint sizex, sizey;
  
        private RGBPixel[,] pbuf;
        
        /// <summary>
        /// Wellenlänge in Pixel. Also die Frequenz dessen.
        /// </summary>
        public float Lambda { get; set; } = 18;

        /// <summary>
        /// Geschwindigkeit mit der die Phasenverschiebung pro iteration verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = 1f;

        public float Winkel { get; set; } = 45f;

        public string Pfad { get; set; } = string.Empty;


        public void Dispose()
        {        
            _image?.Dispose();
        }

        public Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            this.pbuf = new RGBPixel[sizex, sizey];

            if(!string.IsNullOrEmpty(Pfad))
            {
                using(FileStream pngStream = new FileStream(Pfad,FileMode.Open, FileAccess.Read))
                _image = new Bitmap(pngStream);
                FrameDimension dimension = new FrameDimension(_image.FrameDimensionsList[0]);
                _gifframeCount = _image.GetFrameCount(dimension);
            }
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            // Recycle deinen Puffer
            
            // Winkel berechnen in rad
            double wr = this.Winkel / 180d * Math.PI;


            int k = (int)(frame % (ulong)_gifframeCount);
            _image.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
            //Image image = Image.GetInstance(_image, System.Drawing.Imaging.ImageFormat.Bmp);
            Image img = ((Image)_image.Clone());

            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    pbuf[x, y] = new HSVPixel(1, .9f, .9f);
                }
            }

            return Task.FromResult(pbuf);
        }
    }
}
