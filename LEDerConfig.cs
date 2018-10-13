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
        /// <summary>
        /// Bei <c>true</c> sind die Werte in <see cref="SizeX"/> und <see cref="SizeY"/>
        /// nicht relevant bzw. können von der Engine gegeben werden. Bei <c>false</c>
        /// werden die werte für bar genommen.
        /// </summary>
        public bool AutoSize { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        [NonSerialized]
        private IFilter inst;

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