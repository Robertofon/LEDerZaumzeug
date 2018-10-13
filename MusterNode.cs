using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LEDerZaumzeug
{
    public abstract class MusterNode
    {
        /// <summary>
        /// Typname, wie ihn <see cref="Type.GetType"/> verarbeiten kann,
        /// um eine Instanz dieser Klasse als Generator, Filter oder Mixer zu erstellen.
        /// Soll im Serilat vorne stehen - qua Attribut.
        /// </summary>
        [JsonProperty(Order = -2)]
        public string TypeName { get; set; }

        /// <summary>
        /// Kollektion an extra Parametern, die die einzelnen Generatoren oder Filter
        /// benötigen. So kann direkt in den JSON-Level des <see cref="MusterNode"/> und
        /// abkömmlinge zusätzliche Felder hinzugefügt werden. Da sie keine Properties haben,
        /// landen sie qua diesem Attribut hier im Dictionary.
        /// https://stackoverflow.com/questions/15253875/deserialize-json-with-known-and-unknown-fields
        /// https://www.newtonsoft.com/json/help/html/DeserializeExtensionData.htm
        /// </summary>
        [JsonExtensionData]
        private Dictionary<string, JToken> _eParams;

        /// <summary>
        /// Helfermethode, um die Dreiersequenz zu kapseln.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T CreateObjectInstance<T>()
        {
            Type t = Type.GetType(this.TypeName);
            var obj = (T)Activator.CreateInstance(t);
            // Hmmm. vllt. ein bisschen blöd, erst json string draus zu machen, aber geht.
            // Exceptions... könnte passieren.
            string json = JsonConvert.SerializeObject(this._eParams);
            try
            {
                JsonConvert.PopulateObject(json, obj, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    Converters = { new RGBPixelConverter() }
                });
            }
            catch (Exception parsex)
            {
                Console.WriteLine("Zuweisung von zusatzparametern gescheitert bei Instanz von: " + this.TypeName + parsex.ToString());
            }
            //Newtonsoft.Json.Linq.
            //this.Params[].
            return obj;
        }

    }

    [Serializable]
    public class GeneratorNode : MusterNode
    {
        [NonSerialized]
        private IGenerator inst;

        [DebuggerHidden]
        internal IGenerator Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IGenerator>());
            }
        }
    }


    [Serializable]
    public class FilterNode : MusterNode
    {
        [NonSerialized]
        private IFilter inst;

        [DebuggerHidden]
        internal IFilter Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IFilter>());
            }
        }

        public MusterNode Quelle { get; set; }
    }


    [Serializable]
    public class MixerNode : MusterNode
    {
        //public string TypeName { get; set; }

        [NonSerialized]
        private IMixer inst;

        [DebuggerHidden]
        internal IMixer Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IMixer>());
            }
        }

        public IList<MusterNode> Quelle { get; set; } = new List<MusterNode>();
    }



}