using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class ColorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            writer.WriteValue($"#{color.R:x}{color.G:x}{color.B:x}{color.A:x}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            var r = int.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
            var g = int.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
            var b = int.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
            var a = value.Length > 7 ? int.Parse(value.Substring(7, 2), NumberStyles.HexNumber) : 255;
            return new Color(r, g, b, a);
        }
    }
}