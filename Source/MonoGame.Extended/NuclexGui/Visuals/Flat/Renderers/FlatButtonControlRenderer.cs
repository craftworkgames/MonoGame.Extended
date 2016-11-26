using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.NuclexGui.Controls.Desktop;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders button controls in a traditional flat style</summary>
    public class FlatButtonControlRenderer : IFlatControlRenderer<GuiButtonControl>
    {
        /// <summary>Names of the states the button control can be in</summary>
        /// <remarks>
        ///     Storing this as full strings instead of building them dynamically prevents
        ///     any garbage from forming during rendering.
        /// </remarks>
        private static readonly string[] _states =
        {
            "button.disabled",
            "button.normal",
            "button.highlighted",
            "button.depressed"
        };

        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiButtonControl control, IFlatGuiGraphics graphics)
        {
            var controlBounds = control.GetAbsoluteBounds();

            // Determine the style to use for the button
            var stateIndex = 0;
            if (control.Enabled)
                if (control.Depressed)
                    stateIndex = 3;
                else if (control.MouseHovering || control.HasFocus)
                    stateIndex = 2;
                else stateIndex = 1;

            // Draw the button's frame
            graphics.DrawElement(_states[stateIndex], controlBounds);

            // If there's image assigned to the button, draw it into the button
            if (control.Texture != null)
                graphics.DrawImage(controlBounds, control.Texture, control.SourceRectangle);

            // If there's text assigned to the button, draw it into the button
            if (!string.IsNullOrEmpty(control.Text))
                graphics.DrawString(_states[stateIndex], controlBounds, control.Text);
        }
    }
}