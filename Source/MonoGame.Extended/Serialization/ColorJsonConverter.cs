using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class ColorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(ColorHelper.ToHex((Color)value));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            return value.StartsWith("#") ? ColorHelper.FromHex(value) : ColorHelper.FromName(value);
        }
    }
}