using System;
using System.Collections.Generic;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Ein Baum mit Generatoren als Blätter ab <see cref="Root"/>
    /// Dies ist die Exekutor-Klasse.Oder nicht.
    /// </summary>
    public class MusterPipeline
    {
        public MusterNode Root { get; private set; }
    }

    public abstract class MusterNode
    {
    }


    [Serializable]
    public class GeneratorNode : MusterNode
    {
        public string TypeName { get; set; }
        //public IGenerator Gen { get; set; }
    }


    [Serializable]
    public class FilterNode : MusterNode
    {
         public string TypeName { get; set; }
       //[NonSerialized]
        //public IFilter Filt { get; set; }

        public MusterNode Quelle { get; set; } 
    }


    [Serializable]
    public class JoinNode : MusterNode
    {
        public string TypeName { get; set; }

        internal Lazy<IJoin> Join => new Lazy<IJoin>(CreateObjectInstance);

        private IJoin CreateObjectInstance()
        {
            Type t = Type.GetType(this.TypeName);
            IJoin obj = (IJoin)Activator.CreateInstance(t);
            return obj;
        }

        public IList<MusterNode> Quelle { get; set; } = new List<MusterNode>();
    }


}