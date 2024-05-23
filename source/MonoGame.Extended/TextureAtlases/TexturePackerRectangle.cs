using System.Text.Json.Serialization;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerRectangle
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("w")]
        public int Width { get; set; }

        [JsonPropertyName("h")]
        public int Height { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", X, Y, Width, Height);
        }
    }
}
