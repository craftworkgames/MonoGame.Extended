using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    public class BitmapFontProcessorResult
    {
        public BitmapFontProcessorResult(string json)
        {
            TextureAssets = new List<string>();
            Json = json;
        }

        public List<string> TextureAssets { get; private set; }
        public string Json { get; private set; }
    }
}