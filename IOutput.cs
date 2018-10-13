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
        /// <summary>
        /// Bei <c>true</c> sind die Werte in <see cref="SizeX"/> und <see cref="SizeY"/>
        /// nicht relevant bzw. können von der Engine gegeben werden. Bei <c>false</c>
        /// werden die werte für bar genommen.
        /// </summary>
        SizeMode AutoSize { get; set; }

        int SizeX { get; set; }

        int SizeY { get; set; }

        Task Play(RGBPixel[,] pixels);

        Task<OutputInfos> GetInfos();

        Task Initialize(object paramset);
    }
}
