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
        /// <see cref="SizeModes"/> what to enter here in order
        /// to give <see cref="SizeX"/> an <see cref="SizeY"/> a meaning.
        /// </summary>
        SizeModes SizeMode { get; set; }

        int SizeX { get; set; }

        int SizeY { get; set; }

        /// <summary>
        /// Ausspielen der Daten wohin auch immer.
        /// </summary>
        Task Play(RGBPixel[,] pixels);

        Task<OutputInfos> GetInfos();

        Task<bool> Initialize(LEDerConfig cfg);

        /// <summary>
        /// Wird nach <see cref="Initialize(object)"/> aufgerufen, um nach einem Prozess
        /// mit diesen und andern Output-Dimensionen eine neue konsens-Dimension zu setzen.
        /// </summary>
        /// <param name="rechenDimX"></param>
        /// <param name="rechenDimY"></param>
        void SetSize(int rechenDimX, int rechenDimY);
    }
}
