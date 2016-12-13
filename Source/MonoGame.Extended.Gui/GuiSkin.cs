using System.Collections.Generic;

namespace MonoGame.Extended.Gui
{
    public class GuiSkin
    {
        public GuiSkin()
        {
            Templates = new List<GuiControlTemplate>();
        }

        public string Name { get; set; }
        public List<GuiControlTemplate> Templates { get; }
    }

    public class GuiControlTemplate
    {
        public string Type { get; set; }
    }
}