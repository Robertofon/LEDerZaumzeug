using System;
using System.Collections.Generic;

namespace LEDerZaumGUI.PropertyGrid
{
    public enum PropertyValueType
    {
        Unsupported,

        Bool,

        String,

        Enum,

        TextAndHexadecimalEdit,
         
        DetailSettings,

        FixedPossibleValues
    }

    public interface IPropertyContractResolver
    {
        T? GetDataAnnotation<T>(Type targetType, string propertyName)
            where T : Attribute;

        IEnumerable<Attribute> GetDataAnnotations(Type targetType, string propertyName);
    }

    public class DetailSettingsAttribute : Attribute
    {
        public DetailSettingsAttribute()
        {

        }
    }
}
