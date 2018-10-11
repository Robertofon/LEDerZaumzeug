using System;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Ausgabeobjekt. Wird bei Aktivierung im Zaumzeug instantiiert und
    /// überlebt die ganze Zeit bis entweder das Programm beendet /
    /// Zaumzeug disposed wird oder der Output entfernt wird aus der Output-Liste.
    /// </summary>
    public interface IOutput : IDisposable
    {
        Task Play(RGBPixel[,] pixels);

        Task<OutputInfos> GetInfos();

        Task Initialize(object paramset);
    }
}
