namespace MonoGame.Extended.Gui.Controls
{
    public class Label : ContentControl
    {
        public Label()
        {
        }

        public Label(string text = null) 
        {
            Content = text ?? string.Empty;
        }
    }
}