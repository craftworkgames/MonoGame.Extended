using System;
using System.Globalization;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class SizeFJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sizeF = (SizeF) value;
            writer.WriteValue($"{sizeF.Width} {sizeF.Height}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = (string) reader.Value;
            var fields = value.Split(' ');
            var width = float.Parse(fields[0], CultureInfo.InvariantCulture.NumberFormat);
            var height = float.Parse(fields[1], CultureInfo.InvariantCulture.NumberFormat);
            return new SizeF(width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SizeF);
        }
    }
}