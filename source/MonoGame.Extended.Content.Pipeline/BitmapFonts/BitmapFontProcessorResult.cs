using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    public class BitmapFontProcessorResult
    {
        public List<string> TextureAssets { get; private set; }
        public BmfFile FontFile { get; private set; }

        public BitmapFontProcessorResult(BmfFile fontFile)
        {
            FontFile = fontFile;
            TextureAssets = new List<string>();
        }
    }
}
