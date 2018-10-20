using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public interface IFilter
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, ulong frame);

        Task<FilterInfos> GetInfos();
    }
}
