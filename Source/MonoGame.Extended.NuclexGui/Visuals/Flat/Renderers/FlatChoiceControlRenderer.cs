using MonoGame.Extended.NuclexGui.Controls.Desktop;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders choice controls in a traditional flat style</summary>
    public class FlatChoiceControlRenderer : IFlatControlRenderer<GuiChoiceControl>
    {
        /// <summary>Names of the states the choice control can be in</summary>
        /// <remarks>
        ///     Storing this as full strings instead of building them dynamically prevents
        ///     any garbage from forming during rendering.
        /// </remarks>
        private static readonly string[] _states =
        {
            "radio.off.disabled",
            "radio.off.normal",
            "radio.off.highlighted",
            "radio.off.depressed",
            "radio.on.disabled",
            "radio.on.normal",
            "radio.on.highlighted",
            "radio.on.depressed"
        };

        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiChoiceControl control, IFlatGuiGraphics graphics)
        {
            // Determine the index of the state we're going to display
            var stateIndex = control.Selected ? 4 : 0;
            if (control.Enabled)
            {
                if (control.Depressed)
                    stateIndex += 3;
                else
                {
                    if (control.MouseHovering)
                        stateIndex += 2;
                    else stateIndex += 1;
                }
            }

            // Get the pixel coordinates of the region covered by the control on
            // the screen
            var controlBounds = control.GetAbsoluteBounds();
            var width = controlBounds.Width;

            // Now adjust the bounds to a square of height x height pixels so we can
            // render the graphical portion of the choice control
            controlBounds.Width = controlBounds.Height;
            graphics.DrawElement(_states[stateIndex], controlBounds);

            // If the choice has text assigned to it, render it too
            if (!string.IsNullOrEmpty(control.Text))
            {
                // Restore the original width, then subtract the region that was covered by
                // the graphical portion of the control.
                controlBounds.Width = width - controlBounds.Height;
                controlBounds.X += controlBounds.Height;

                // Draw the text that was assigned to the choice control        
                graphics.DrawString(_states[stateIndex], controlBounds, control.Text);
            }
        }
    }
}