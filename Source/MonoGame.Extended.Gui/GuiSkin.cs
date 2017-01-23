using System.Collections.Generic;

namespace MonoGame.Extended.Gui
{
    public class GuiSkin
    {
        public GuiSkin()
        {
            Templates = new Dictionary<string, GuiControlStyle>();
        }

        public string Name { get; set; }
        public Dictionary<string, GuiControlStyle> Templates { get; }
    }
}