using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using LEDerZaumzeug;

namespace LEDerZaumzeugLib
{
    /// <summary>
    /// Reflektiert vorhandene und erweiterte Klassen
    /// </summary>
    public sealed class LEDerZaumReflector
    {
        public LEDerZaumReflector()
        {
        }

        public void Init()
        {
            Assembly thisasm = this.GetType().Assembly;
            Reflect(thisasm);
        }

        public void AddAssembly(Assembly a)
        {
            Reflect(a);
        }

        private void Reflect(Assembly asm)
        {
            var types = asm.GetExportedTypes().Where(t => t.IsClass).ToList();
            this.OutputTypes.AddRange(types.Where(t=>t.GetInterfaces().Contains(typeof(IOutput))));
            this.MixerTypes.AddRange(types.Where(t=>t.GetInterfaces().Contains(typeof(IMixer))));
            this.FilterTypes.AddRange(types.Where(t=>t.GetInterfaces().Contains(typeof(IFilter))));
            this.GeneratorTypes.AddRange(types.Where(t=>t.GetInterfaces().Contains(typeof(IGenerator))));

            this.Outputs.AddRange( Enrich(this.OutputTypes, "O") );
            this.Mixers.AddRange( Enrich(this.MixerTypes, "M") );
            this.Filters.AddRange( Enrich(this.FilterTypes, "F") );
            this.Generators.AddRange( Enrich(this.GeneratorTypes, "G") );
        }

        private IEnumerable<LInfo> Enrich(List<Type> inp, string art)
        {
            return inp.Select(enrich);

            LInfo enrich(Type t)
            {
                string name = t.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? t.Name;
                string desc = t.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? String.Empty;
                string typeId = t.FullName;
                return new LInfo(name, desc, typeId, t, art);
            }
        }

        public List<Type> OutputTypes { get; } = new List<Type>();
        public List<Type> MixerTypes { get; } = new List<Type>();
        public List<Type> FilterTypes { get; } = new List<Type>();
        public List<Type> GeneratorTypes { get; } = new List<Type>();
        public List<LInfo> Outputs { get; } = new List<LInfo>();
        public List<LInfo> Mixers { get; } = new List<LInfo>();
        public List<LInfo> Filters { get; } = new List<LInfo>();
        public List<LInfo> Generators { get; } = new List<LInfo>();
    }

    public struct LInfo
    {
        public LInfo(string name, string desc, string typeId, Type type, string art)
        {
            Name = name;
            Desc = desc;
            TypeId = typeId;
            Type = type;
            Art = art;
        }

        public string Name { get; }
        public string Desc { get; }
        public string TypeId { get; }
        public Type Type { get; }
        public string Art { get; }
        public override string ToString()
        {
            return Name;
        }
    }
}
