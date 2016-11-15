using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class ColorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color) value;
            var rx = $"{color.R:x2}";
            var gx = $"{color.G:x2}";
            var bx = $"{color.B:x2}";
            var ax = $"{color.A:x2}";
            var colorHex = $"#{rx}{gx}{bx}{ax}";
            Debug.Assert(colorHex.Length == 9);
            writer.WriteValue(colorHex);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return ColorHelper.FromHex((string) reader.Value);
        }
    }
}