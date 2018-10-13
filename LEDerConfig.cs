using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Hauptkonfig für das Programmm.
    /// Hier sind die Ausgaben konfiguriert und Startoptionen.
    /// </summary>
    [Serializable]
    public class LEDerConfig
    {
        /// <summary>
        /// Zeit pro quasi Dia.
        /// </summary>
        public TimeSpan SeqShowTime { get; set; }

        public IList<OutputNode> Outputs { get; set; } = new List<OutputNode>();

    }

    [Serializable]
    public class OutputNode : MusterNode
    {
        [NonSerialized]
        private IOutput inst;

        [DebuggerHidden]
        internal IOutput Inst
        {
            get
            {
                return this.inst ?? (inst = this.CreateObjectInstance<IOutput>());
            }
        }
    }

}