using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        
        Task<RGBPixel[,]> GenPattern(Size size, long frame);
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
    public interface IJoin
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

    class Program
    {
        static async Task Main(string[] args)
        {
            test();
            LEDerConfig lederconfig = null;
            Console.WriteLine("LEDerZaumzeug!\nPixelgenerator meiner Wahl.");
            string cfgf = "config.json";
            Console.WriteLine("lese Konfig: " + cfgf);
            using (var stream = File.OpenText(cfgf))
            {
                string cstr = stream.ReadToEnd();
                lederconfig = JsonConvert.DeserializeObject<LEDerConfig>(cstr, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    //SerializationBinder = knownTypesBinder
                });


            }

            using (var pixelator = new LEDerZaumZeug(lederconfig, null))
            {
                await pixelator.StartAsync();

            }
        }

        static void test()
        { 
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
                        TypeName = "Multi",
                        Quelle =
                        {
                            new FilterNode()
                            {
                                TypeName = "Invert",
                                Quelle = new GeneratorNode() { TypeName="Solid" }
                            },
                            new FilterNode()
                            {
                                TypeName = "Tiefpass",
                                Quelle = new GeneratorNode() { TypeName="Solid" }
                            },
                            new FilterNode()
                            {
                                TypeName = "Tiefpass",
                                Quelle = new FilterNode()
                                {
                                    TypeName ="Flt2",
                                    Quelle = new GeneratorNode() { TypeName="Solid" }
                                }
                            }
                        }
                    }
                }
            };

            KnownTypesBinder knownTypesBinder = new KnownTypesBinder
            {
                KnownTypes = new List<Type> { typeof(FilterNode), typeof(GeneratorNode), typeof(JoinNode), typeof(OutputNode) }
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
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder
            });


            var ldc = new LEDerConfig()
            {
                SeqShowTime = TimeSpan.FromSeconds(33),
                Outputs =
                {
                    new OutputNode()
                    {
                        TypeName="LEDerZaumzeug.Tpm2NetOutput",
                        AutoSize = false,
                        SizeX = 8,
                        SizeY = 12,
                        Cfg =
                        {
                            ["Order"] = "SNV_TL"
                        }
                    }
                }
            };
            string gq = Newtonsoft.Json.JsonConvert.SerializeObject(ldc, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                SerializationBinder = knownTypesBinder
            });

            ITraceWriter tw = new MemoryTraceWriter();
            var restorcfg = Newtonsoft.Json.JsonConvert.DeserializeObject<LEDerConfig>(gq, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                TraceWriter = tw
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
