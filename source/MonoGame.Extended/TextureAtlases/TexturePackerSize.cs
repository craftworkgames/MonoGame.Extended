using System.Text.Json.Serialization;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerSize
    {
        [JsonPropertyName("w")]
        public int Width { get; set; }

        [JsonPropertyName("h")]
        public int Height { get; set; }

        public override string ToString()
        {
            return $"{Width} {Height}";
        }
    }
}
