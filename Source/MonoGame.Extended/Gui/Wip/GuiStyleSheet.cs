using System.Collections.Generic;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Wip
{
    public class GuiLayout
    {
        public string StyleSheet { get; set; }
        public GuiControl[] Controls { get; set; }
    }

    public class GuiStyleSheet
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
        public Dictionary<string, JObject> Styles { get; set; }
    }
}