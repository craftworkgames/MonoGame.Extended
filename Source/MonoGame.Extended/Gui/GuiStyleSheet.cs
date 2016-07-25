using System.Collections.Generic;

namespace MonoGame.Extended.Gui
{
    public class GuiStyleSheet
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
        public Dictionary<string, GuiStyle> Styles { get; set; }
    }
}