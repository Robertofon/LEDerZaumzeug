using LEDerZaumzeug.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Mixer
{
    /// <summary>
    /// Fade-Zusammenführung.
    /// Erlaubt linear zwischen Input0 und Input1 zu überblenden.
    /// </summary>
    [Description("Fade-Mixer")]
    public class LinearFade : IMixer
    {
        /// <summary>
        /// Faktor zwischen eingabe 1 und eingabe 0.
        /// </summary>
        /// <value>Zwischen 0 (erste) und 1 (zweite).</value>
        public float Fade { get; set; }

        public Task Initialize(MatrixParams matrixParameters)
        {
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> Mix(IList<RGBPixel[,]> sources, ulong frame)
        {
            float f0 = this.Fade.LimitTo(0f, 1f);
            float f1 = 1f - f0;
            if( sources.Count == 2)
            {
                (int x, int y) = sources[0].Dim();
                var res = new RGBPixel[x, y];
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        res[i, j] = sources[0][i, j] * f0 + sources[1][i, j] * f1;
                return Task.FromResult(res);
            }
            
            if(sources.Count == 1)
            {
                return Task.FromResult(sources[0].Clone2( (RGBPixel a) => a * f0 ));
            }

            throw new ArgumentException("Unterstütze nicht mehr als zwei Eingaben und auch nicht 0");
        }
        void IDisposable.Dispose()
        {
        }

    }
}
