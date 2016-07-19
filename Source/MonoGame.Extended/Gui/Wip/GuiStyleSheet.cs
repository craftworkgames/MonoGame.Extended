using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Wip
{
    public class GuiLayout
    {
        public GuiStyleSheet StyleSheet { get; set; }
        public GuiControl[] Controls { get; set; }

        public T FindControl<T>(string name)
            where T : GuiControl
        {
            return Controls.FirstOrDefault(c => c.Name == name) as T;
        }
    }

    public class GuiStyleSheet
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
        public Dictionary<string, GuiTemplate> Styles { get; set; }
    }
}