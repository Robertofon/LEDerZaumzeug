using System;
using System.Collections.Generic;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Serialisierungsklasse respektive Datenklasse. Stellt ein Komplettes Programm von
    /// einzelnen Mustern dar. Format des Pixelgenerators.
    /// Unterschied zu <see cref="SeqItemNode"/>: Hier ist eine Sequenz oder Liste von Mustern.
    /// Der o.g. enthält jeweils nur ein Muster, das mit einem Namen bezeichnet ist.
    /// </summary>
    [Serializable]
    public class PixelProgram
    {
        public string Name { get; set; }

        public string Autor { get; set; }

        public Dictionary<string, string> MetaInfo { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Liste von Generatorbäumen.
        /// </summary>
        public List<SeqItemNode> Seq { get; private set; } = new List<SeqItemNode>();
    }
}