using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public interface IFilter : IDisposable
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame);

        Task<FilterInfos> GetInfos();
    }
}
