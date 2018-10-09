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
    [Description("Vollfarbe")]
    public class SolidColor : IGenerator
    {
        /// <summary>
        /// Parameter to set color.
        /// </summary>
        public RGBPixel Color { get; set; }

        public void Dispose()
        {            
        }

        public Task<RGBPixel[,]> GenPattern(Size size)
        {
            var p = new RGBPixel[size.Width, size.Height];
            for( int x= 0; x < size.Width; x++)
            {
                for( int y= 0; y < size.Height; y++)
                p[x,y] = this.Color;
            }

            return Task.FromResult(p);
        }

        public Task<GeneratorInfos> GetInfos()
        {
            return Task.FromResult(new GeneratorInfos());
        }

        public async Task Initialize(MatrixParams matrixParameters)
        {
            return;
        }
    }
}
