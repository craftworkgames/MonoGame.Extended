namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabel : GuiControl
    {
        public GuiLabel()
        {
        }

        public GuiLabel(GuiSkin skin, string text = null) 
            : base(skin)
        {
            Text = text ?? string.Empty;
        }
    }
}