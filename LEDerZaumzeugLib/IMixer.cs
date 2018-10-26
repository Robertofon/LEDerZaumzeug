using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Vereinigung von Pixelströmen oder Operatoren in das Ganze.
    /// Hier können Mehrere Generatoren oder Filter zusammengeführt/ 
    /// zusammengemixt werden
    /// </summary>
    public interface IMixer : IDisposable
    {
        Task Initialize(MatrixParams matrixParameters);

        /// <summary>
        /// Mixt n Datenströme Pixelseiten zusammen.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        Task<RGBPixel[,]> Mix(IList<RGBPixel[,]> sources, ulong frame);
    }
}
