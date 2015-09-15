using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    public class BitmapFontProcessorResult
    {
        public BitmapFontProcessorResult(BitmapFontFile fontFile)
        {
            FontFile = fontFile;
            TextureAssets = new List<string>();
        }

        public List<string> TextureAssets { get; private set; }
        public BitmapFontFile FontFile { get; private set; }
    }
}