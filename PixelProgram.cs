using System;
using System.Collections.Generic;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Serialisierungsklasse respektive Datenklasse. Stellt ein Komplettes Programm von
    /// einzelnen Mustern dar. Format des Pixelgenerators.
    /// </summary>
    [Serializable]
    public class PixelProgram
    {
        public Dictionary<string, string> Meta { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Liste von Generatorbäumen.
        /// </summary>
        public List<MusterNode> Seq { get; private set; } = new List<MusterNode>();
    }
}