using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class SizeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sizeF = (Size) value;
            writer.WriteValue($"{sizeF.Width} {sizeF.Height}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var values = reader.ReadAsMultiDimensional<int>();

            if(values.Length == 2)
                return new Size(values[0], values[1]);

            if (values.Length == 1)
                return new Size(values[0], values[0]);

            throw new FormatException("Invalid Size property value");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Size);
        }
    }
}
