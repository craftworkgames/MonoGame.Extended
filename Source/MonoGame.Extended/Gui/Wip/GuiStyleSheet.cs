using System.Collections.Generic;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Wip
{
    public class GuiLayout
    {
        public GuiStyleSheet StyleSheet { get; set; }
        public GuiControl[] Controls { get; set; }
    }

    public class GuiStyleSheet
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
        public Dictionary<string, GuiTemplate> Styles { get; set; }
    }
}