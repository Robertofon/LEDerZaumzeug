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

        public Punkt(long x, long y)
        {
            this.x=(uint) x; this.y=(uint) y;
        }

        public uint x;
        public uint y;
    }

}