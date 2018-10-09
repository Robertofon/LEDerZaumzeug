using System;
using System.Collections.Generic;
using System.Text;

namespace LEDerZaumzeug.Extensions
{
    public static class ArrayExtensions
    {
        public static (int,int) Dim<T>(this T[,] arr)
        {
            return (arr.GetLength(0), arr.GetLength(1));
        }
    }
}
