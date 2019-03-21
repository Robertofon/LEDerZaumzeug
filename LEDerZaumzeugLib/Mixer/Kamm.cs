using LEDerZaumzeug.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Mixer
{
    /// <summary>
    /// Kamm-Zusammenführung.
    /// Blendet eine Zeile Input0 und eine Zeile Input1 ineinander.
    /// Zusätzlich sind die Rechten und linken respektive oben und unten
    /// miteinander verbunden und somit Input0 oder Input1.
    /// Mehr inputs werden ignoriert, bei einem Input wird geklont.
    /// </summary>
    [Description("Kamm-Mixer")]
    public class Kamm : IMixer
    {
        /// <summary>
        /// Orientierung
        /// </summary>
        /// <value><c>Hori</c> für horizontal und <c>Verti</c> für Vertikal.</value>
        public Ori Orientierung { get; set; }

        public Task Initialize(MatrixParams matrixParameters)
        {
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> Mix(IList<RGBPixel[,]> sources, ulong frame)
        {
            if( sources.Count >= 2)
            {
                (int x, int y) = sources[0].Dim();
                var res = new RGBPixel[x, y];
                for (int i = 0; i < x; i++)
                {  
                    for (int j = 0; j < y; j++)
                    {
                        int zz, zm, k;
                        if(Orientierung == Ori.Horiz)
                        {    
                            zz = i;
                            zm = x-1;
                            k = j;
                        }
                        else
                        {
                            zz = j;
                            zm = y-1;
                            k = i;
                        }

                        if(zz==0) // Rand 0
                        {
                            res[i,j]= sources[0][i, j];
                        }
                        else if ( zz== zm) // Rand 1
                        {
                            res[i,j]= sources[1][i, j];
                        }
                        else // Kamm in mod2
                        {
                            res[i, j] = sources[k%2][i, j];
                        }
                    }
                }
                return Task.FromResult(res);
            }
            
            if(sources.Count == 1)
            {
                return Task.FromResult(sources[0].Clone2( (RGBPixel a) => a ));
            }

            throw new ArgumentException("Unterstütze nicht mehr als zwei Eingaben und auch nicht 0");
        }
        void IDisposable.Dispose()
        {
        }

        public enum Ori
        {
            Horiz,
            Verti
        }

    }
}
