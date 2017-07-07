using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class RangeJsonConverter<T> : JsonConverter where T : IComparable<T>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var range = (Range<T>) value;

            var formatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            writer.WriteWhitespace(" ");
            writer.WriteStartArray();
            serializer.Serialize(writer, range.Min);
            serializer.Serialize(writer, range.Max);
            //writer.WriteValue(range.Min);
            //writer.WriteValue(range.Max);
            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = reader.ReadAsMultiDimensional<T>();

            if (values.Length == 2)
            {
                if (values[0].CompareTo(values[1]) < 0)
                    return new Range<T>(values[0], values[1]);

                return new Range<T>(values[1], values[0]);
            }
                

            if (values.Length == 1)
                return new Range<T>(values[0], values[0]);

            throw new InvalidOperationException("Invalid range");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Range<T>);
        }
    }
}