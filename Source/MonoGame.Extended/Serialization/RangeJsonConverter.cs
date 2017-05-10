using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class RangeJsonConverter<T> : JsonConverter where T : IComparable<T>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = reader.ReadAsMultiDimensional<T>();

            if (values.Length == 2)
                return new Range<T>(values[0], values[1]);

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