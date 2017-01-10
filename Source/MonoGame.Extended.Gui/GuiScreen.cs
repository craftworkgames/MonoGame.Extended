using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public class GuiScreen
    {
        public GuiScreen()
        {
            Controls = new GuiControlCollection();
        }

        public GuiControlCollection Controls { get; }
    }
}