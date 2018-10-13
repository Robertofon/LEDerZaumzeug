using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public interface IFilter
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<RGBPixel[,]> Filter(RGBPixel[,] pixels, long frame);

        Task<FilterInfos> GetInfos();
    }
}
