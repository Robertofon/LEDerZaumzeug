using System.Drawing;

namespace LEDerZaumzeug
{
    public class OutputInfos
    {
        public SubSample DesiredSubSamplemode { get; set; }

        /// <summary>
        /// Liefert die Ausgabedimension (gewünschte) zurück. 
        /// Falls nicht relevant oder nicht herausfindbar, soll <c>null</c> 
        /// als Don't care geliefert werden.
        /// </summary>
        public SizeF? Dim { get; set; }


    }
}