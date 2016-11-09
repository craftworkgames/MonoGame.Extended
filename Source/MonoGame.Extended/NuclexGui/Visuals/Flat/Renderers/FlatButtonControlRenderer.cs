using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders button controls in a traditional flat style</summary>
    public class FlatButtonControlRenderer : IFlatControlRenderer<Controls.Desktop.GuiButtonControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(Controls.Desktop.GuiButtonControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            // Determine the style to use for the button
            int stateIndex = 0;
            if (control.Enabled)
            {
                if (control.Depressed)
                {
                    stateIndex = 3;
                }
                else if (control.MouseHovering || control.HasFocus)
                {
                    stateIndex = 2;
                }
                else {
                    stateIndex = 1;
                }
            }

            // Draw the button's frame
            graphics.DrawElement(states[stateIndex], controlBounds);

            // If there's text assigned to the button, draw it into the button
            if (!string.IsNullOrEmpty(control.Text))
            {
                graphics.DrawString(states[stateIndex], controlBounds, control.Text);
            }
        }

        /// <summary>Names of the states the button control can be in</summary>
        /// <remarks>
        ///   Storing this as full strings instead of building them dynamically prevents
        ///   any garbage from forming during rendering.
        /// </remarks>
        private static readonly string[] states = new string[] {
      "button.disabled",
      "button.normal",
      "button.highlighted",
      "button.depressed"
    };
    }
}
