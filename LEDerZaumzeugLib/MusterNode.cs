using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LEDerZaumzeug
{
    public abstract class BaseNode
    {

    }

    /// <summary>
    /// Repräsentiert einen Baum mit Namen und einem Wurzelknoten
    /// als <see cref="MusterNode"/>. Existiert nur, um weitere
    /// Metadaten in Pro Muster-Kompositions-Item einzubauen.
    /// </summary>
    [Serializable]
    public class SeqItemNode : BaseNode
    {
        /// <summary>
        /// Name des Musterbaums
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Wurzel des Muster-Generator-Baums. Hier anfangen
        /// </summary>
        public MusterNode Start { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public abstract class MusterNode : BaseNode
    {
        private static readonly NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
  
        /// <summary>
        /// Typname, wie ihn <see cref="Type.GetType"/> verarbeiten kann,
        /// um eine Instanz dieser Klasse als Generator, Filter oder Mixer zu erstellen.
        /// Soll im Serilat vorne stehen - qua Attribut.
        /// </summary>
        [JsonProperty(Order = -2)]
        public string TypeName { get; set; }

#pragma warning disable 0649 // standard wert - nie was anderes zugewiesen.
        /// <summary>
        /// Kollektion an extra Parametern, die die einzelnen Generatoren oder Filter
        /// benötigen. So kann direkt in den JSON-Level des <see cref="MusterNode"/> und
        /// abkömmlinge zusätzliche Felder hinzugefügt werden. Da sie keine Properties haben,
        /// landen sie qua diesem Attribut hier im Dictionary.
        /// https://stackoverflow.com/questions/15253875/deserialize-json-with-known-and-unknown-fields
        /// https://www.newtonsoft.com/json/help/html/DeserializeExtensionData.htm
        /// </summary>
        [JsonExtensionData]
        private JObject _extparams;
#pragma warning restore

        /// <summary>
        /// Erzeugt das Objekt des Typs in <see cref="TypeName"/> und
        /// appliziert die vorhandenen Zusatzparameter von <see cref="_eParams"/>.
        /// </summary>
        protected T CreateObjectInstance<T>()
        {
            log.Info($"CreateObjectInstance - suche '{this.TypeName}'.");
            Type t = Type.GetType(this.TypeName);
            var obj = (T)Activator.CreateInstance(t);
            // JSON an neu erzeugtes objekt applizieren.
            // https://stackoverflow.com/questions/52792214/how-to-apply-jsonextensiondata-dictionarystring-jtoken-to-another-object-wi
            // Exceptions... passieren und sagen, dass Werte nicht zugewiesen werden können (Jo, Syntax Jungs!!)
            try
            {
                // wir haben extraparameter
                if(_extparams != null)
                {
                    log.Info($"Populiere Objekt '{obj.GetType().Name} mit Parametern");
                    new JsonSerializer()
                    {
                        Converters = { new RGBPixelConverter() },
                        MissingMemberHandling = MissingMemberHandling.Error,
                    }.Populate(_extparams.CreateReader(), obj);
                }
            }
            catch (Exception parsex)
            {
                log.Error(parsex, $"Zuweisung von Zusatzparametern gescheitert bei Instanz von: '{this.TypeName}'. Zusatzparameter:");
                log.Error(JsonConvert.SerializeObject(_extparams, Formatting.Indented));
            }

            return obj;
        }

        public override string ToString()
        {
            return "MN: " + this.TypeName;
        }
    }

    /// <summary>
    /// Generator knoten des Muster-Generierungs Baums in seiner Serialsierungsform.
    /// <see cref="Inst"/> wird via <see cref="MusterNode.TypeName"/> instantiiert.
    /// Generatoren sind die Blätter des Baumes und somit die eigentlichen Pixelbild-Quellen.
    /// </summary>
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

    /// <summary>
    /// Repräsentiert einen Serialisierten Filterknoten.
    /// Daher mit <see cref="MusterNode.TypeName"/> und als 
    /// einzelne Quelle, aus der er sich Speist gibt es <see cref="Quelle"/>.
    /// </summary>
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


    /// <summary>
    /// Repräsentiert einen serialisierten Knoten eines Mixers.
    /// Ein Zusammenführer von mehreren Quellen. Daher mit  
    /// <see cref="MusterNode.TypeName"/> und als die mehreren Quellen, 
    /// aus denen er sich Speist gibt es <see cref="Quelle"/>.
    /// </summary>
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