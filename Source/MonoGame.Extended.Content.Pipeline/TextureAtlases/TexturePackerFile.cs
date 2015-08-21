using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerFile
    {
        [JsonProperty("frames")]
        public List<TexturePackerFrame> Frames { get; set; }

        [JsonProperty("meta")]
        public TexturePackerMeta Meta { get; set; }
    }
}