using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerSize
    {
        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Width, Height);
        }
    }
}