using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace LEDerZaumzeug
{
    public class SerialisierungsFabrik
    {
        /// <summary>
        /// Vorschlag von Newtonsoft für einen Type-binder.
        /// </summary>
        internal class KnownTypesBinder : ISerializationBinder
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

        private static KnownTypesBinder knownTypesBinder = new KnownTypesBinder
        {
            KnownTypes = new List<Type> { typeof(SeqItemNode), typeof(FilterNode), typeof(GeneratorNode), typeof(MixerNode), typeof(OutputNode) }
        };

        public static async Task<PixelProgram> ReadProgramFromStreamAsync(Stream stream)
        {
            var reader = new StreamReader(stream);
            {
                string str = await reader.ReadToEndAsync();
                return ReadProgramFromString(str);
            }
        }

        public static PixelProgram ReadProgramFromString(string str)
        {
            return JsonConvert.DeserializeObject<PixelProgram>(str, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder
            });
        }


        public static async Task<LEDerConfig> ReadConfigFromStreamAsync(Stream stream)
        {
            var reader = new StreamReader(stream);
            {
                string str = await reader.ReadToEndAsync();
                return ReadConfigFromString(str);
            }
        }

        public static LEDerConfig ReadConfigFromString(string str)
        {
            return JsonConvert.DeserializeObject<LEDerConfig>(str, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder
            });
        }


        public static string WriteProgramToString(PixelProgram pgrm)
        {
            return JsonConvert.SerializeObject(pgrm, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                SerializationBinder = knownTypesBinder
            });
        }

        public static T Clone<T>(T node)
        {
            string s = Serialize(node);

            return DeSerialize<T>(s);
        }

        public static string Serialize<T>(T node)
        {
            return JsonConvert.SerializeObject(node, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder
            });
        }

        public static T DeSerialize<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = knownTypesBinder
            });
        }
    }
}
