using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization;

public class RectangleFJsonConverter: JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var rect = (RectangleF)value;
        writer.WriteValue($"{rect.Left} {rect.Top} {rect.Width} {rect.Height}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var values = reader.ReadAsMultiDimensional<float>();
        return new RectangleF(values[0], values[1], values[2], values[3]);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RectangleF);
    }
}
