using LEDerZaumzeug.Extensions;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
        private static Regex htmlcolor = new Regex("^#[0-9A-Fa-f]?(?<h>[0-9A-Fa-f]{3})$|^#[0-9A-Fa-f]{0,2}(?<h>[0-9A-Fa-f]{6})$");
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

        /// <summary>
        /// Unterstützt das Lesen aus HTML-Color definitionen. Siehe regex.
        /// Also Dinge wie #AABBCC für RGB oder #33FF55cc für ARGB definitionen 
        /// oder die Kürzere Variante #444. Dabei werden die Hexwerte jeweils in 0..255 umgewandelt
        /// und per Dreisatz auf 0f..1f abgebildet. Alpha wird nicht unterstützt.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static RGBPixel? FromHtmlColor(string s)
        {
            var matches = htmlcolor.Match(s);
            if (matches.Success)
            {
                var htmlc = matches.Groups["h"].ToString();
                int dist = htmlc.Length == 3 ? 1 : 2;
                IFormatProvider f = CultureInfo.InvariantCulture;
                if ( uint.TryParse(htmlc.Substring(0*dist,dist), NumberStyles.HexNumber, f, out uint r)
                    && uint.TryParse(htmlc.Substring(1 * dist, dist), NumberStyles.HexNumber, f, out uint g)
                    && uint.TryParse(htmlc.Substring(2 * dist, dist), NumberStyles.HexNumber, f, out uint b))
                {
                    float norm = htmlc.Length == 3 ? 15f : 255f;
                    return new RGBPixel(r / norm, g / norm, b / norm);
                }
            }

            return null;
        }

        public static RGBPixel P0 => new RGBPixel(0f,0f,0f);

        public static RGBPixel P1 => new RGBPixel(1f,1f,1f);

        
        public static bool TryParse(string str, IFormatProvider prov, out RGBPixel pixel)
        {
            if ((str.First() == '[' && str.Last() == ']') || (str.First() == '(' && str.Last() == ')'))
            {
                string[] komp = str.Substring(1, str.Length - 2).Split('/');
                if (komp.Length != 3)
                {
                    pixel = default;
                    return false;
                }

                NumberStyles style = NumberStyles.Float;
                float r, g, b;
                if (float.TryParse(komp[0], style, prov, out r) && float.TryParse(komp[1], style, prov, out g) && float.TryParse(komp[2], style, prov, out b))
                {
                    pixel = new RGBPixel(r, g, b);
                    return true;
                }
            }

            pixel = default;
            return false;
        }

        /// <summary>
        /// Macht daraus einen String - gut für Debugger-Zwecke.
        /// </summary>
        public override string ToString()
        {
            return $"[{R}/{G}/{B}]";
        }

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

        /// <summary>
        /// Überladener == operator.
        /// </summary>
        public static bool operator==(RGBPixel a, RGBPixel b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Überladener != operator.
        /// </summary>
        public static bool operator!=(RGBPixel a, RGBPixel b)
        {
            return !a.Equals(b);
        }

    }
}
