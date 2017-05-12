using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    /// <summary>
    /// This converter handles the different spellings of Center.
    /// </summary>
    public class GuiAlignmentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(HorizontalAlignment))
            {
                var value = reader.Value.ToString();

                if (value == "Center" || string.Equals(value, "Centre", StringComparison.OrdinalIgnoreCase))
                    return HorizontalAlignment.Centre;

                return serializer.Deserialize<HorizontalAlignment>(reader);
            }

            if (objectType == typeof(VerticalAlignment))
            {
                var value = reader.Value.ToString();

                if (value == "Center" || string.Equals(value, "Centre", StringComparison.OrdinalIgnoreCase))
                    return VerticalAlignment.Centre;

                return serializer.Deserialize<VerticalAlignment>(reader);
            }

            throw new InvalidOperationException($"Invalid value for '{objectType.Name}'");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HorizontalAlignment) || objectType == typeof(VerticalAlignment);
        }
    }
}