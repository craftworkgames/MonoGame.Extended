namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>Control that draws a block of text</summary>
    public class GuiLabelControl : GuiControl
    {
        /// <summary>Text to be rendered in the control's frame</summary>
        public string Text;

        /// <summary>
        /// Gets or sets the style to be used with this label when drawing.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If you are using the FlatGuiVisualizer, this style
        ///         corresponds to the 'Frames' used in 
        ///         <see cref="MonoGame.Extended.NuclexGui.Visuals.Flat.FlatGuiGraphics"/>
        ///         The style can be customized in the skin's json file.
        ///     </para>
        /// </remarks>
        public string Style { get; set; }

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