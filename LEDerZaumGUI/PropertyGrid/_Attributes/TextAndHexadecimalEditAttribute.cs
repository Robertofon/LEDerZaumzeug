using System;

namespace LEDerZaumGUI.PropertyGrid._Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextAndHexadecimalEditAttribute : Attribute
    {
        public string EncodingWebNamePropertyName { get; set; }

        public TextAndHexadecimalEditAttribute(string encodingWebNameProperty)
        {
            this.EncodingWebNamePropertyName = encodingWebNameProperty;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LinkAttribute : Attribute
    {
    }
}