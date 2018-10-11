using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Generators
{
    /// <summary>
    /// Generator für vollflächig eine Farbe. <see cref="Color"/> setzt die Farbe.
    /// </summary>
    [Description("Vollfarbe in genau einer Variante")]
    public class SolidColor : IGenerator
    {
        private uint sizex, sizey;

        /// <summary>
        /// Parameter to set color.
        /// </summary>
        public RGBPixel Color { get; set; }

        public void Dispose()
        {            
        }

        public Task<RGBPixel[,]> GenPattern(long frame)
        {
            var p = new RGBPixel[sizex, sizey];
            for( int x= 0; x < sizex; x++)
            {
                for( int y= 0; y < sizey; y++)
                p[x,y] = this.Color;
            }

            return Task.FromResult(p);
        }

        public Task<GeneratorInfos> GetInfos()
        {
            return Task.FromResult(new GeneratorInfos());
        }

        public async Task Initialize(MatrixParams mparams)
        {
            this.sizex = mparams.SizeX;
            this.sizey = mparams.SizeY;
            return;
        }
    }
}
