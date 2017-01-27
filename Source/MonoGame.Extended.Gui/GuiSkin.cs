using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public class GuiSkin
    {
        public GuiSkin()
        {
            Templates = new Dictionary<string, GuiControlStyle>();
        }

        public BitmapFont DefaultFont => Fonts.Values.FirstOrDefault();
        public Dictionary<string, BitmapFont> Fonts { get; set; }
        public string Name { get; set; }
        public Dictionary<string, GuiControlStyle> Templates { get; }
        public Dictionary<string, TextureAtlas> TextureAtlases { get; set; }
    }
}