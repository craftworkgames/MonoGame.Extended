namespace MonoGame.Extended.Gui.Controls
{
    public abstract class ContentControl : Control
    {
        public object Content { get; set; }
        public bool HasContent => Content == null;
    }
}