namespace LEDerZaumzeug
{
    /// <summary>
    /// Werttyp. Struktur für einen Punkt mit X- und Y-Koordinate.
    /// </summary>
    public struct Punkt
    {
        public Punkt(uint x, uint y)
        {
            this.x=x; this.y=y;
        }

        public uint x;
        public uint y;
    }

}