using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEDerZaumzeug.Extensions
{
    public static class LedExtensions
    {
        public static (int,int) Dim<T>(this T[,] arr)
        {
            return (arr.GetLength(0), arr.GetLength(1));
        }

        /// <summary>
        /// Klont und mappt ein zweidimensionales Feld vom Typ Ti in den Typ To
        /// ebenfalls als zweidimensionales Feld. Liefert das geklonte und modifizierte
        /// Ergebnis zurück.
        /// </summary>
        /// <typeparam name="To"></typeparam>
        /// <typeparam name="Ti"></typeparam>
        /// <param name="arr">Eingabefeld.</param>
        /// <param name="mapfn"></param>
        /// <returns>Geklontes und gemapptes Feld.</returns>
        public static To[,] Clone2<To,Ti>(this Ti[,] arr, Func<Ti,To> mapfn)
        {
            (int x, int y) = arr.Dim();
            To[,] res = new To[x,y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    res[i, j] = mapfn(arr[i, j]);
                }
            }

            return res;
        }

        public static bool IsHoriz(this PixelArrangement pa)
        {
            return new[] { 
                PixelArrangement.SNH_BL, PixelArrangement.SNH_BR, 
                PixelArrangement.SNH_TL, PixelArrangement.SNH_TR,
                PixelArrangement.LNH_BL, PixelArrangement.LNH_BR, 
                PixelArrangement.LNH_TL, PixelArrangement.LNH_TR }.Contains(pa);
        }

        public static bool IsVert(this PixelArrangement pa)
        {
            return !IsHoriz(pa);
        }

        public static bool IsSnakeWise(this PixelArrangement pa)
        {
            return pa.ToString().Contains("SN");
        }

        public static bool IsStartTop(this PixelArrangement pa)
        {
            return pa.ToString().Contains("T");
        }

        public static bool IsStartRight(this PixelArrangement pa)
        {
            return pa.ToString().Contains("R");
        }

    }
}
