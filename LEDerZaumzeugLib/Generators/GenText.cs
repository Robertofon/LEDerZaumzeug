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
using SixLabors.Shapes;
using SixLabors.Fonts;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator, der Text durchrollen lässt. Sorry kein R2L
    /// </summary>
    public class GenText : IGenerator
    {
        private static ILogger _log = NLog.LogManager.GetCurrentClassLogger();
        private Image<Rgba32> _image;
        private uint sizex, sizey;
        private float _tpos;
        private RGBPixel[,] pbuf;
        private readonly string[] fontpfade = 
        {
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        };

        /// <summary>
        /// Geschwindigkeit mit der der Text verschoben wird.
        /// </summary>
        public float Geschwindigkeit { get; set; } = .5f;

        /// <summary>
        /// Text, der angezeigt werden soll.
        /// </summary>
        public string Text { get; set; } = "LEDerZaumzeug";

        /// <summary>
        /// Schriftgröße 8..40.
        /// </summary>
        public float Size { get; set; } = 14;

        /// <summary>
        /// Style der Schrift. Bold, Italic, SemiBold, Normal.
        /// </summary>
        public FontStyle Style { get; set; }

        /// <summary>
        /// Fontfamilie. Sowas wie Serif oder Sans Serif.
        /// </summary>
        public string FontFamily { get; set; } = "Arial";

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

            Font font =null;
            var fonts = new FontCollection();
            foreach(var f in fontpfade)
            {
                if( System.IO.File.Exists(f))
                {
                    fonts.Install(f);
                }
            }
            try{
                font = SystemFonts.CreateFont(this.FontFamily, this.Size, this.Style);
            }
            catch(Exception d){}
            SizeF sz = TextMeasurer.Measure(this.Text, new RendererOptions(font));
            Size _tsize = new Size(Convert.ToInt32(sz.Width + 1), Convert.ToInt32(sz.Height + 1));
            this._image = new Image<Rgba32>(_tsize.Width, _tsize.Height);
            _image.Mutate(ctx =>
            {
                //ctx.Fill(Rgba32.Black);
                ctx.DrawText(this.Text, font, Brushes.Solid(Rgba32.White), Pens.Solid(Rgba32.Beige,0), PointF.Empty);
            });

            _tpos = _image.Width/2;
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> GenPattern(ulong frame)
        {
            int y_st = (int)sizey/2 - (_image.Height / 2)+1;
            int y_ed = y_st + _image.Height;
            int x_ed = _image.Width + (int)_tpos;

            // Recycle deinen Puffer
            // Bild übertragen...
            for( int x= 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    if (y >= y_st && y < y_ed && x>=_tpos && x < x_ed)
                    {
                        Rgba32 pxl = _image[x-(int)_tpos, y-y_st];
                        pbuf[x, y] = new RGBPixel(pxl.R/255f, pxl.G/255f, pxl.B/255f);
                    }
                    else
                    {
                        pbuf[x, y] = RGBPixel.P0;
                    }
                }
            }

            // Text scrollen lassen und resetten
            _tpos -= this.Geschwindigkeit;
            if (_tpos + _image.Width + 6 < 0)
                _tpos = _image.Width / 2;
            return Task.FromResult(pbuf);
        }

    }
}
