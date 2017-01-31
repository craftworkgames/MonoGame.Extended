using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerRegion
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("frame")]
        public TexturePackerRectangle Frame { get; set; }

        [JsonProperty("rotated")]
        public bool IsRotated { get; set; }

        [JsonProperty("trimmed")]
        public bool IsTrimmed { get; set; }

        [JsonProperty("spriteSourceSize")]
        public TexturePackerRectangle SourceRectangle { get; set; }

        [JsonProperty("sourceSize")]
        public TexturePackerSize SourceSize { get; set; }

        [JsonProperty("pivot")]
        public TexturePackerPoint PivotPoint { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Filename, Frame);
        }
    }
}