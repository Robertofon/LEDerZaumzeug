using System.Collections.Generic;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Ein Baum mit Generatoren als Blätter
    /// </summary>
    internal class MusterPipeline
    {
        public MusterNode Root { get; private set; }
    }

    internal abstract class MusterNode
    {

    }

    internal class GeneratorNode : MusterNode
    {
        public IGenerator Gen { get; set; }
    }


    internal class FilterNode : MusterNode
    {
        public IFilter Filt { get; set; }

        public MusterNode Quelle { get; set; } 
    }


    internal class JoinNode : MusterNode
    {
        public IJoins Join { get; set; }

        public IList<MusterNode> Quelle { get; set; } 
    }


}