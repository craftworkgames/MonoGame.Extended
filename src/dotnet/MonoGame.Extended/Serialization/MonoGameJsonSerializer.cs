using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public sealed class MonoGameJsonSerializer : JsonSerializer
    {
        public MonoGameJsonSerializer(ContentManager contentManager, string contentPath, NullValueHandling nullValueHandling = NullValueHandling.Include)
        {
            Converters.Add(new ColorJsonConverter());
            Converters.Add(new HslColorJsonConverter());
            Converters.Add(new RangeJsonConverter<int>());
            Converters.Add(new RangeJsonConverter<float>());
            Converters.Add(new RangeJsonConverter<HslColor>());
            Converters.Add(new ThicknessJsonConverter());
            Converters.Add(new TextureAtlasJsonConverter(contentManager, contentPath));
            Converters.Add(new Size2JsonConverter());

            ContractResolver = new ShortNameJsonContractResolver();
            NullValueHandling = nullValueHandling;
            Formatting = Formatting.Indented;
        }
    }
}