using Newtonsoft.Json;
using System;

namespace LEDerZaumzeug
{
    public class RGBPixelConverter : JsonConverter<RGBPixel>
    {

        public override void WriteJson(JsonWriter writer, RGBPixel value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override RGBPixel ReadJson(JsonReader reader, Type objectType, RGBPixel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            if(RGBPixel.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, out RGBPixel px))
                return px;
            else
            {
                RGBPixel? html = RGBPixel.FromHtmlColor(s);
                if (html != null)
                    return html.Value;
            }

            throw new FormatException($"RGB-Pixel nicht lesbar: '{s}'");
        }
    }



}