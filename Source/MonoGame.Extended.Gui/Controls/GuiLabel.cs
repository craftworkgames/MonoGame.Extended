namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabel : GuiControl
    {
        public GuiLabel()
        {
        }

        public GuiLabel(string text = null) 
        {
            Text = text ?? string.Empty;
        }
    }
}