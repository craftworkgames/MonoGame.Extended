namespace MonoGame.Extended.Gui.Controls
{
    public class GuiPanel : GuiControl
    {
        public GuiPanel()
        {
            Controls = new GuiControlCollection(this);
        }

        public GuiControlCollection Controls { get; set; }
    }
}