using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    public class TexturePackerRectangle
    {
        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", X, Y, Width, Height);
        }
    }
}