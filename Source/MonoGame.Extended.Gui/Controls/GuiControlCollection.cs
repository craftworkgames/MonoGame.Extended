namespace MonoGame.Extended.Gui.Controls
{
    public class GuiControlCollection : GuiElementCollection<GuiControl, GuiControl>
    {
        public GuiControlCollection()
            : base(null)
        {
        }

        public GuiControlCollection(GuiControl parent)
            : base(parent)
        {
        }
    }
}