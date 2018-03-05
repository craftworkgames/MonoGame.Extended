using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class AlignmentConverter : JsonConverter
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

                HorizontalAlignment alignment;

                if (Enum.TryParse(value, true, out alignment))
                    return alignment;
            }

            if (objectType == typeof(VerticalAlignment))
            {
                var value = reader.Value.ToString();

                if (value == "Center" || string.Equals(value, "Centre", StringComparison.OrdinalIgnoreCase))
                    return VerticalAlignment.Centre;

                VerticalAlignment alignment;

                if (Enum.TryParse(value, true, out alignment))
                    return alignment;
            }

            throw new InvalidOperationException($"Invalid value for '{objectType.Name}'");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HorizontalAlignment) || objectType == typeof(VerticalAlignment);
        }
    }
}