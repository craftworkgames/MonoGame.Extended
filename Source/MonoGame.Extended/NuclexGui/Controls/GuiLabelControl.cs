namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>Control that draws a block of text</summary>
    public class GuiLabelControl : GuiControl
    {
        /// <summary>Text to be rendered in the control's frame</summary>
        public string Text;

        /// <summary>Initializes a new label control with an empty string</summary>
        public GuiLabelControl() : this(string.Empty)
        {
        }

        /// <summary>Initializes a new label control</summary>
        /// <param name="text">Text to be printed at the location of the label control</param>
        public GuiLabelControl(string text)
        {
            Text = text;
        }
    }
}