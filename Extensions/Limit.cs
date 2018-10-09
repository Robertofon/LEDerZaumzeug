using System;
using System.Collections.Generic;
using System.Text;

namespace LEDerZaumzeug.Extensions
{
    /// <summary>
    /// Klasse stellt erweiterungsmethoden bereit um an ordinalen Typen
    /// limits einzuhalten. Also "lass das nicht größer werden als..."
    /// </summary>
    /// <example>
    /// int i = 43, j=i.LimitTo(2,44);
    /// </example>
    public static class Limit
    {
        public static T LimitTo<T>(this T inValue, T lowerLimit, T upperLimit) where T : IComparable
        {
            if (inValue.CompareTo(lowerLimit) < 0)
                return lowerLimit;
            if (inValue.CompareTo(upperLimit) > 0)
                return upperLimit;
            return inValue;
        }

        public static T LimitTo<T>(this T inValue, T upperLimit) where T : IComparable
        {
            if (inValue.CompareTo(upperLimit) > 0)
                return upperLimit;
            return inValue;
        }

        public static double LimitTo_NoNan(this double inValue, double upperLimit)
        {
            if (double.IsNaN(upperLimit))
                return inValue;
            if (inValue.CompareTo(upperLimit) > 0)
                return upperLimit;
            return inValue;
        }
    }
}
