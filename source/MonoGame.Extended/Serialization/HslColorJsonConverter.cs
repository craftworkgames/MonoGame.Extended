using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class HslColorJsonConverter : JsonConverter
    {
        private readonly ColorJsonConverter _colorConverter = new ColorJsonConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = ((HslColor) value).ToRgb();
            _colorConverter.WriteJson(writer, color, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var color = (Color)_colorConverter.ReadJson(reader, objectType, existingValue, serializer);
            return HslColor.FromRgb(color);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HslColor);
        }
    }
}