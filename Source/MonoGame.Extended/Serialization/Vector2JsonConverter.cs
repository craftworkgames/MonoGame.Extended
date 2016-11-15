using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class Vector2JsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector2 = (Vector2) value;
            writer.WriteValue($"{vector2.X} {vector2.Y}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = (string) reader.Value;
            var fields = value.Split(' ');
            var x = float.Parse(fields[0], CultureInfo.InvariantCulture.NumberFormat);
            var y = float.Parse(fields[1], CultureInfo.InvariantCulture.NumberFormat);
            return new Vector2(x, y);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }
    }
}