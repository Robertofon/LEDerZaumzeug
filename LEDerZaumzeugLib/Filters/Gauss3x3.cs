using LEDerZaumzeug.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Filters
{
    /// <summary>
    /// Überträgt das Eingangsbild in ein gleichgroßes Ausgangsbild.
    /// Dabei wird ein Gauß'scher Weichzeichner verwendet mit einem 3x3-Kern.
    /// </summary>
    public class Gauss3x3 : FaltungBasis
    {
        protected override float[] InitKernel()
        {
            // 1 2 1          || 0  1  2
            // 2 4 2          || 3  4  5  
            // 1 2 1          || 6  7  8

            //                || 0  1  2   3  4  5    6  7  8
            var kern = new float[]{ 1, 2, 1,  2, 4, 2 ,  1, 2, 1 };
            return kern;
        }
    }
}
