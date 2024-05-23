using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerFile
    {
        [JsonPropertyName("frames")]
        public List<TexturePackerRegion> Regions { get; set; }

        [JsonPropertyName("meta")]
        public TexturePackerMeta Metadata { get; set; }
    }
}
