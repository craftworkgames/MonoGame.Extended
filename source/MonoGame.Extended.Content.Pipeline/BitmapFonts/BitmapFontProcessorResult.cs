using System.Collections.Generic;
using MonoGame.Extended.Content.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    public class BitmapFontProcessorResult
    {
        public List<string> TextureAssets { get; private set; }
        public BitmapFontFileContent FontFile { get; private set; }

        public BitmapFontProcessorResult(BitmapFontFileContent fontFile)
        {
            FontFile = fontFile;
            TextureAssets = new List<string>();
        }
    }
}
