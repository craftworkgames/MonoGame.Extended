using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerFrame
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("frame")]
        public TexturePackerRectangle Frame { get; set; }

        [JsonProperty("rotated")]
        public bool Rotated { get; set; }

        [JsonProperty("trimmed")]
        public bool Trimmed { get; set; }

        [JsonProperty("spriteSourceSize")]
        public TexturePackerRectangle SpriteSourceSize { get; set; }

        [JsonProperty("sourceSize")]
        public TexturePackerSize SourceSize { get; set; }

        [JsonProperty("pivot")]
        public TexturePackerPoint Pivot { get; set; }
    }
}