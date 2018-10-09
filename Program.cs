using System;
using System.Collections.Generic;
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

    public interface IGenerator : IDisposable
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<GeneratorInfos> GetInfos();

        Task<RGBPixel[,]> GenPattern();
    }

    public interface IFilter
    {
        Task Initialize(MatrixParams matrixParameters);

        Task<RGBPixel[,]> Filter(RGBPixel[,] pixels);

        Task<FilterInfos> GetInfos();
    }

    /// <summary>
    /// Vereinigung von Pixelströmen oder Operatoren in das Ganze.
    /// Hier können Mehrere Generatoren oder Filter zusammengeführt werden-
    /// </summary>
    public interface IJoins
    {
        Task Initialize(MatrixParams matrixParameters);
        Task Join(IList<RGBPixel[,]> sources);
    }

    public enum SubSample
    {
        S1x1,
        S2x2,
        S4x4,
        S8x8,
    }

    /// <summary>
    /// Hauptklasse, instanzieren um zu arbeiten.
    /// </summary>
    public class LEDerZaumZeug : IDisposable
    {
        LEDerConfig config;
        MusterSequenz sequenz;
        private MusterPipeline activePipeline;
        private IEnumerable<IOutput> outputs;

        public void Dispose()
        {
            StopAsync();
        }

        public Task StartAsync()
        {

        }

        public Task StopAsync()
        {

        }




    }


    public struct RGBPixel
    {
        float R, G, B;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
