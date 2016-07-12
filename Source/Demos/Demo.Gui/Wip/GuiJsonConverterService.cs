using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Gui.Wip
{
    public class GuiJsonConverterService
    {
        private readonly TextureAtlas _textureAtlas;
        private readonly Dictionary<string, BitmapFont> _bitmapFonts;

        public GuiJsonConverterService(TextureAtlas textureAtlas, IEnumerable<BitmapFont> bitmapFonts)
        {
            _textureAtlas = textureAtlas;
            _bitmapFonts = bitmapFonts.ToDictionary(f => f.Name);
        }

        public TextureRegion2D GetTextureRegion(string name)
        {
            return _textureAtlas[name];
        }

        public BitmapFont GetFont(string name)
        {
            return _bitmapFonts[name];
        }
    }
}