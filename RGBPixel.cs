using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Simpler RGB-Pixel-Typ über <see cref="float"/>. Ideal
    /// um damit weiche Übergägne zu rechnen und später zu skalieren.
    /// Kann nebenbei auch Operatoren und HLS oder Graustufen.
    /// Gleichheit unterstützt struct intrinsisch!
    /// </summary>
    public struct RGBPixel
    {
        public float R, G, B;

        /// <summary>
        /// Konstruktor zur initialisierung mit Werten.
        /// </summary>
        /// <param name="r">Rot</param>
        /// <param name="g">Grün</param>
        /// <param name="b">Blau</param>
        public RGBPixel(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static RGBPixel P0 => new RGBPixel(0f,0f,0f);

        public static RGBPixel P1 => new RGBPixel(1f,1f,1f);

        /// <summary>
        /// Beschneidet den Wertebereich des Pixels, um die Komponenten
        /// R, G und B auf 0.0 .. 1.0 zu bekommen.
        /// </summary>
        /// <returns>Ein <see cref="RGBPixel"/> bei dem alle Werte zwischen 0 und 1 (inkll.) sind.</returns>
        public RGBPixel Clip()
        {
            return new RGBPixel(R.LimitTo(0f, 1f), G.LimitTo(0f, 1f), B.LimitTo(0f, 1f));
        }

        /// <summary>
        /// Invert-Operator. Spiegelt das Pixel an 0.5. Sodass aus 0.0 1.0 wird und aus 0.4 0.6 sowie aus -7 +6 und umgekehrt
        /// </summary>
        /// <param name="p">Pixel zu invertieren.</param>
        /// <returns>Invertiertes Pixel</returns>
        public static RGBPixel operator~(RGBPixel p)
        {
            return P1 - p;
        }

        /// <summary>
        /// Unärer negationsoperator. Macht aus 5.0 -5.0 und aus -0.4 +0.4.
        /// </summary>
        /// <param name="p">Pixel zu negieren.</param>
        /// <returns>Negiertes Pixel.</returns>
        public static RGBPixel operator -(RGBPixel p)
        {
            return new RGBPixel(-p.R, -p.G, -p.B);
        }

        public static RGBPixel operator -(RGBPixel a, RGBPixel b)
        {
            return new RGBPixel(a.R - b.R, a.G - b.G, a.B - b.B);
        }

        public static RGBPixel operator +(RGBPixel a, RGBPixel b)
        {
            return new RGBPixel(a.R + b.R, a.G + b.G, a.B + b.B);
        }

        public static RGBPixel operator *(RGBPixel a, RGBPixel b)
        {
            return new RGBPixel(a.R * b.R, a.G * b.G, a.B * b.B);
        }

        public static RGBPixel operator /(RGBPixel a, RGBPixel b)
        {
            return new RGBPixel(a.R / b.R, a.G / b.G, a.B / b.B);
        }

        /// <summary>
        /// Skalare Multiplikation um einen einfachen floag-Wert.
        /// </summary>
        /// <param name="p">Pixel zu modifizieren.</param>
        /// <param name="f">Faktor, der draufmultipliziert wird.</param>
        /// <returns></returns>
        public static RGBPixel operator *(RGBPixel p, float f)
        {
            return new RGBPixel(p.R * f, p.G * f, p.B * f);
        }

    }
}
