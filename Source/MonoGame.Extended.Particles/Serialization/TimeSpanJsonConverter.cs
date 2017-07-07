using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Particles.Serialization
{
    public class TimeSpanJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var timeSpan = (TimeSpan) value;
            writer.WriteValue(timeSpan.TotalSeconds);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(double))
            {
                var seconds = (double) reader.Value;
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.Zero;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }
    }
}