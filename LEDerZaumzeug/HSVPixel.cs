using System;
using System.Collections.Generic;
using System.Text;

namespace LEDerZaumzeug
{
    /// <summary>
    /// HSV-Implementierung nach https://de.wikipedia.org/wiki/HSV-Farbraum
    /// Problem: Mischung zweier Farben (gelb 100% in Form von rot+grün je 100%) ist in der wahrgenommenen
    /// Helligkeit stärker als rot 100% oder grün 100% allein, denn es leuchten 2 LEDs.
    /// Korrekt wäre, wenn man die rezipierte Helligkeit (lumen) der Farben mit verwursten würde,
    /// und somit auf Helligkeit normieren würde. Zukunft?
    /// </summary>
    public struct HSVPixel
    {
        /// <summary>
        /// Hue - Farbigkeit des Pixels (0..360)
        /// </summary>
        public float H;
        public float S;
        public float V;

        public static readonly HSVPixel P0 = new HSVPixel(0,0,0);

        public HSVPixel(float hue, float saturation, float value)
        {
            this.H = hue;
            this.S = saturation;
            this.V = value;
        }

        public static HSVPixel FromRGB(RGBPixel color)
        {
            float hue, saturation, value;
            float max = Math.Max(color.R, Math.Max(color.G, color.B));
            float min = Math.Min(color.R, Math.Min(color.G, color.B));

            if(max==color.R)
            {
                hue = 60f * (0 + ((color.G - color.B) / (max - min)));
            }
            else if(max==color.G)
            {
                hue = 60f * (2 + ((color.B - color.R) / (max - min)));
            }
            else if(max==color.B)
            {
                hue = 60f * (4 + ((color.R - color.G) / (max - min)));
            }
            else // if(min==max)
            {
                hue = 0;
            }

            if( hue < 0f)
            {
                hue += 360f;
            }
            
            saturation = (max == 0) ? 0 : (max-min) / max;
            value = max;
            return new HSVPixel(hue, saturation, value);
        }

        public static implicit operator RGBPixel(HSVPixel p)
        {
            return RGBFromHSV(p.H, p.S, p.V);
        }

        public static RGBPixel RGBFromHSV(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);

            float v = (value);
            float p = (value * (1 - saturation));
            float q = (value * (1 - f * saturation));
            float t = (value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return new RGBPixel(v, t, p);
                case 1:
                    return new RGBPixel(q, v, p);
                case 2:
                    return new RGBPixel(p, v, t);
                case 3:
                    return new RGBPixel(p, q, v);
                case 4:
                    return new RGBPixel(t, p, v);
                default:
                    return new RGBPixel(v, p, q);
            }
        }

        public override string ToString()
        {
            return $"HSV({H}°,{S},{V})";
        }
    }
}
