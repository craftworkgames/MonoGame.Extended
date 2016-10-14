using System;
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
            return ColorExtensions.FromHex(value);
        }
    }
}