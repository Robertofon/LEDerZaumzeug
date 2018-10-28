using Avalonia.Markup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LEDerZaumGUI
{
    /// <summary>
    /// Converter kann ein Skalar in ein Array[ Skalar ] verwandeln.
    /// Nötig, da der TreeView immer <see cref="IEnumerable{T}"/> sehen will.
    /// </summary>
    public class SingularListConverter : IValueConverter
    {
        private static Lazy<SingularListConverter> _inst = new Lazy<SingularListConverter>();

        public static SingularListConverter Inst
        {
            get => _inst.Value;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
