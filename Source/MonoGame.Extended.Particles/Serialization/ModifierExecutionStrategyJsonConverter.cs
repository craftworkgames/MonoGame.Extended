using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ModifierExecutionStrategyJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = JToken.Load(reader).ToObject<string>();
            return ParticleModifierExecutionStrategy.Parse(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ParticleModifierExecutionStrategy) 
                || objectType.GetTypeInfo().BaseType == typeof(ParticleModifierExecutionStrategy);
        }
    }
}