using LEDerZaumzeug.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Invertiert ein Bild indem Werte von 0..1 auf 1..0 umgedreht werden. 
    /// </summary>
    public class InvertFilter : IFilter
    {
        public Task<RGBPixel[,]> Filter(RGBPixel[,] pixels)
        {
            (int w, int h) = pixels.Dim();
            var res = new RGBPixel[w,h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    res[x, y] = (~pixels[x, y]).Clip();
                }
            }

            return Task.FromResult(res);
        }

        public Task<FilterInfos> GetInfos()
        {
            throw new NotImplementedException();
        }

        public Task Initialize(MatrixParams matrixParameters)
        {
            throw new NotImplementedException();
        }
    }
}
