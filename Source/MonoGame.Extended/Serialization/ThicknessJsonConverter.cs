using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class ThicknessJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var thickness = (Thickness)value;
            writer.WriteValue($"{thickness.Left} {thickness.Top} {thickness.Right} {thickness.Bottom}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = reader.ReadAsMultiDimensional<int>();
            return Thickness.FromValues(values);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Thickness);
        }
    }
}