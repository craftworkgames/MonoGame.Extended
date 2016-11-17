using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiScreen
    {
        public GuiScreen()
        {
            Controls = new GuiControlCollection(null);
        }

        public GuiControlCollection Controls { get; }
    }
}