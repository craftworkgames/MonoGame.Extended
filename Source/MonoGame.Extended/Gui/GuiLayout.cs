using System.Linq;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public class GuiLayout
    {
        public TextureAtlas TextureAtlas { get; set; }
        public GuiControl[] Controls { get; set; }

        public T FindControl<T>(string name)
            where T : GuiControl
        {
            return Controls.FirstOrDefault(c => c.Name == name) as T;
        }
    }
}