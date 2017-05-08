using System;
using MonoGame.Extended.Particles.Profiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ProfileJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = jObject.GetValue("type").ToObject<string>();

            switch (type)
            {
                case "circle":
                    return jObject.ToObject<CircleProfile>(serializer);
                case "ring":
                    return jObject.ToObject<RingProfile>(serializer);
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Profile);
        }
    }
}