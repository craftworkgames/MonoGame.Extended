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
}
