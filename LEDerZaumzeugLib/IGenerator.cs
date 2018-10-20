using System;
using System.Drawing;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public interface IGenerator : IDisposable
    {
        Task Initialize(MatrixParams matrixParameters);

      
        Task<RGBPixel[,]> GenPattern(ulong frame);
    }
}
