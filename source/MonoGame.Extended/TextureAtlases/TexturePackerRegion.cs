using System.Text.Json.Serialization;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerRegion
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("frame")]
        public TexturePackerRectangle Frame { get; set; }

        [JsonPropertyName("rotated")]
        public bool IsRotated { get; set; }

        [JsonPropertyName("trimmed")]
        public bool IsTrimmed { get; set; }

        [JsonPropertyName("spriteSourceSize")]
        public TexturePackerRectangle SourceRectangle { get; set; }

        [JsonPropertyName("sourceSize")]
        public TexturePackerSize SourceSize { get; set; }

        [JsonPropertyName("pivot")]
        public TexturePackerPoint PivotPoint { get; set; }

        public override string ToString()
        {
            return $"{Filename} {Frame}";
        }
    }
}
