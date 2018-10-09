using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        Task<RGBPixel[,]> GenPattern(Size size);
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
        Task<RGBPixel[,]> Join(IList<RGBPixel[,]> sources);
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
        private LEDerConfig config;
        private PixelProgram sequenz;
        private MusterPipeline activePipeline;
        private IEnumerable<IOutput> outputs;

        public void Dispose()
        {
            Stop();
        }

        public void StartAsync()
        {

        }

        public void Stop()
        {

        }




    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LEDerZaumzeug!\nPixelgenerator meiner Wahl.");

            var prg = new PixelProgram()
            {
                Meta =
                {
                    ["ABC"] = "DEF"
                },
                Seq =
                {
                    new JoinNode()
                    {
                        JoinName = "Multi",
                        Quelle =
                        {
                            new FilterNode()
                            {
                                FilterName = "Invert",
                                Quelle = new GeneratorNode() { GeneratorName="Solid" }
                            },
                            new FilterNode()
                            {
                                FilterName = "Tiefpass",
                                Quelle = new GeneratorNode() { GeneratorName="Solid" }
                            },
                            new FilterNode()
                            {
                                FilterName = "Tiefpass",
                                Quelle = new FilterNode()
                                {
                                    FilterName ="Flt2",
                                    Quelle = new GeneratorNode() { GeneratorName="Solid" }
                                }
                            }
                        }
                    }
                }
            };

            KnownTypesBinder knownTypesBinder = new KnownTypesBinder
            {
                KnownTypes = new List<Type> { typeof(FilterNode), typeof(GeneratorNode), typeof(JoinNode) }
            };

            //XmlSerializer xserializer = new XmlSerializer(typeof(JoinNode));
            //xserializer.Serialize(Console.Out, prg);
            var g = Newtonsoft.Json.JsonConvert.SerializeObject( prg, new JsonSerializerSettings()
            {
                TypeNameHandling =TypeNameHandling.Auto,
                Formatting =Formatting.Indented,
                SerializationBinder = knownTypesBinder
            });
            Console.WriteLine(g);
            var restor = Newtonsoft.Json.JsonConvert.DeserializeObject<PixelProgram>(g, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = knownTypesBinder
            });

        }

        public class KnownTypesBinder : ISerializationBinder
        {
            public IList<Type> KnownTypes { get; set; }

            public Type BindToType(string assemblyName, string typeName)
            {
                Type type = KnownTypes.SingleOrDefault(t => t.Name == typeName);
                return type;
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }
    }
}
