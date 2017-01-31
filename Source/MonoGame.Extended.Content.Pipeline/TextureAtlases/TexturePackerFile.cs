using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerFile
    {
        [JsonProperty("frames")]
        public List<TexturePackerRegion> Regions { get; set; }

        [JsonProperty("meta")]
        public TexturePackerMeta Metadata { get; set; }
    }
}