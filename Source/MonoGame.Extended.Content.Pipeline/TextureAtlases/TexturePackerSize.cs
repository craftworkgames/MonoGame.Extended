using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerSize
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        public override string ToString()
        {
            return $"{Width} {Height}";
        }
    }
}