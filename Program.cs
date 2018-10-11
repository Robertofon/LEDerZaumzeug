using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{

    class Program
    {
        private const string StdPath = "Mini.ledp";
        //private const string StdPath = "Sequenz.ledp";
        private const string CfgPath = "config.json";

        static async Task Main(string[] args)
        {
            //test();
            LEDerConfig lederconfig = null;
            Console.WriteLine("LEDerZaumzeug!\nPixelgenerator meiner Wahl.");
            Console.WriteLine("lese Konfig: " + CfgPath);
            using (var stream = File.OpenRead(CfgPath))
            {
                lederconfig = await SerialisierungsFabrik.ReadConfigFromStreamAsync(stream);
            }

            PixelProgram programmsequenz = null;
            using (Stream stream = File.OpenRead(StdPath))
            {
                programmsequenz = await SerialisierungsFabrik.ReadProgramFromStreamAsync(stream);
            }

            using (var pixelator = new LEDerZaumZeug(lederconfig, programmsequenz))
            {
                await pixelator.StartAsync();

            }
        }

        static void test()
        {

            var prg = new PixelProgram()
            {
                MetaInfo =
                {
                    ["ABC"] = "DEF",
                    ["Autor"] = "Hansi",
                },
                Seq =
                {
                    new MixerNode()
                    {
                        TypeName = "LEDerZaumzeug.LinearFade",
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

            SerialisierungsFabrik.KnownTypesBinder knownTypesBinder = new SerialisierungsFabrik.KnownTypesBinder
            {
                KnownTypes = new List<Type> { typeof(FilterNode), typeof(GeneratorNode), typeof(MixerNode), typeof(OutputNode) }
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

    }
}
