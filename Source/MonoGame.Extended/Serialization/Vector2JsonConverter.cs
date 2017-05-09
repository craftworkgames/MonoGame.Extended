using System;
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
            var values = reader.ReadAsMultiDimensional<float>();

            if(values.Length == 2)
                return new Vector2(values[0], values[1]);

            if (values.Length == 1)
                return new Vector2(values[0]);

            throw new InvalidOperationException("Invalid Vector2");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }
    }
}