using System;
using System.Drawing;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public interface IGenerator : IDisposable
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<GeneratorInfos> GetInfos();
        
        Task<RGBPixel[,]> GenPattern(long frame);
    }
}
