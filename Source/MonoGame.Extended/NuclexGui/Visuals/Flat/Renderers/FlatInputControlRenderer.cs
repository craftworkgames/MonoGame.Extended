using Microsoft.Xna.Framework;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders text input controls in a traditional flat style</summary>
    public class FlatInputControlRenderer : IFlatControlRenderer<GuiInputControl>, IOpeningLocator
    {
        /// <summary>Style from the skin this renderer uses</summary>
        private const string _style = "input.normal";

        // TODO: Find a better solution than remembering the graphics interface here
        //   Otherwise the renderer could try to renderer when no frame is being drawn.
        //   Also, the renderer makes the assumption that all drawing happens through
        //   one graphics interface only.

        /// <summary>Graphics interface we used for the last draw call</summary>
        private IFlatGuiGraphics _graphics;

        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiInputControl control, IFlatGuiGraphics graphics)
        {
            var controlBounds = control.GetAbsoluteBounds();

            // Draw the control's frame and background
            graphics.DrawElement(_style, controlBounds);

            using (graphics.SetClipRegion(controlBounds))
            {
                var text = control.Text ?? string.Empty;

                // Amount by which the text will be moved within the input box in
                // order to keep the caret in view even when the text is wider than
                // the input box.
                float left = 0;

                // Only scroll the text within the input box when it has the input
                // focus and the caret is being shown.
                if (control.HasFocus)
                {
                    // Find out where the cursor is from the left end of the text
                    var stringSize = graphics.MeasureString(
                        _style, controlBounds, text.Substring(0, control.CaretPosition)
                    );

                    // TODO: Renderer should query the size of the control's frame
                    //   Otherwise text will be visible over the frame, which might look bad
                    //   if a skin uses a frame wider than 2 pixels or in a different color
                    //   than the text.
                    while (stringSize.Width + left > controlBounds.Width)
                        left -= controlBounds.Width/10.0f;
                }

                // Draw the text into the input box
                controlBounds.X += left;
                graphics.DrawString(_style, controlBounds, control.Text);

                // If the input box is in focus, also draw the caret so the user knows
                // where characters will be inserted into the text.
                if (control.HasFocus)
                    if (control.MillisecondsSinceLastCaretMovement%500 < 250)
                        graphics.DrawCaret(
                            "input.normal", controlBounds, control.Text, control.CaretPosition
                        );
            }

            // Let the control know that we can provide it with additional informations
            // about how its text is being rendered
            control.OpeningLocator = this;
            _graphics = graphics;
        }

        /// <summary>
        ///     Calculates which opening between two letters is closest to a position
        /// </summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <param name="text">Text in which the opening will be looked for</param>
        /// <param name="position">
        ///     Position to which the closest opening will be found,
        ///     should be in absolute coordinates
        /// </param>
        /// <returns>The index of the opening closest to the provided position</returns>
        public int GetClosestOpening(
            RectangleF bounds, string text, Vector2 position
        )
        {
            return _graphics.GetClosestOpening("input.normal", bounds, text, position);
        }
    }
}