using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEDerZaumzeug.Extensions
{
    public static class LedExtensions
    {
        public static (int, int) Dim<T>(this T[,] arr)
        {
            return (arr.GetLength(0), arr.GetLength(1));
        }

        /// <summary>
        /// Erstellt ein neues 2D-Feld derselben Dimension wie das übergebene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">Eingabefeld.</param>
        /// <returns>Neues Feld.</returns>
        public static T[,] NewSameDim<T>(this T[,] arr)
        {
            (int x, int y) = arr.Dim();
            T[,] res = new T[x, y];
            return res;
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
        public static To[,] Clone2<To, Ti>(this Ti[,] arr, Func<Ti, To> mapfn)
        {
            (int x, int y) = arr.Dim();
            To[,] res = new To[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    res[i, j] = mapfn(arr[i, j]);
                }
            }

            return res;
        }

        /// <summary>
        /// mappt ein zweidimensionales Feld vom Typ Ti in den Typ To
        /// ebenfalls als zweidimensionales Feld. Beide müssen existieren.
        /// und dieselbe Dimension haben. Liefert das erste und modifizierte
        /// Ergebnis zurück.
        /// </summary>
        /// <typeparam name="To"></typeparam>
        /// <typeparam name="Ti"></typeparam>
        /// <param name="arr1">Eingabefeld und Resultat.</param>
        /// <param name="arr2">Eingabefeld 2.</param>
        /// <param name="mapfn"></param>
        /// <returns>Geklontes und gemapptes Feld.</returns>
        public static To[,] MapInplace<To>(this To[,] arr1, To[,] arr2, Func<To, To, To> mapfn)
        {
            if (arr1.Dim() != arr2.Dim())
            {
                throw new ArgumentException("Dims ungleich");
            }

            var (x, y) = arr1.Dim();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    arr1[i, j] = mapfn(arr1[i, j], arr2[i, j]);
                }
            }

            return arr1;
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

        public static (int, int) EnsureArray2D<T>(this T[,] bsp, ref T[,] res)
        {
            // wiederverwenden desselben Speichers -> jetzt dynamisch anlegen
            (int, int) dim = bsp.Dim();
            if (res == null || res.Dim() != dim)
            {
                (int w, int h) = dim;
                res = new T[w, h];
            }
            return dim;
        }

    }
}
