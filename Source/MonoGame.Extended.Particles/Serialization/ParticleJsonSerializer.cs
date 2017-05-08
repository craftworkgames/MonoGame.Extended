using System;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Particles.Serialization
{
    public sealed class ParticleJsonSerializer : JsonSerializer
    {
        public ParticleJsonSerializer(ITextureRegionService textureRegionService)
        {
            Converters.Add(new Vector2JsonConverter());
            Converters.Add(new Size2JsonConverter());
            Converters.Add(new ColorJsonConverter());
            Converters.Add(new TextureRegion2DJsonConverter(textureRegionService));
            Converters.Add(new ProfileJsonConverter());
            Converters.Add(new TimeSpanJsonConverter());
            Converters.Add(new RangeJsonConverter<int>());
            Converters.Add(new RangeJsonConverter<float>());
            ContractResolver = new ShortNameJsonContractResolver();
            Formatting = Formatting.Indented;
        }
    }

    public class RangeJsonConverter<T> : JsonConverter where T : IComparable<T>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            var min = jArray[0].ToObject<T>();
            var max = jArray[1].ToObject<T>();
            return new Range<T>(min, max);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Range<T>);
        }
    }
}
