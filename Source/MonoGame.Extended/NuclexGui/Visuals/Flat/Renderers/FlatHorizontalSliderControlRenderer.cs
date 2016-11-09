using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders horizontal sliders in a traditional flat style</summary>
    public class FlatHorizontalSliderControlRenderer : IFlatControlRenderer<Controls.Desktop.GuiHorizontalSliderControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(Controls.Desktop.GuiHorizontalSliderControl control, IFlatGuiGraphics graphics
        )
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            float thumbWidth = controlBounds.Width * control.ThumbSize;
            float thumbX = (controlBounds.Width - thumbWidth) * control.ThumbPosition;

            graphics.DrawElement("rail.horizontal", controlBounds);

            RectangleF thumbBounds = new RectangleF(controlBounds.X + thumbX, controlBounds.Y, thumbWidth, controlBounds.Height);

            if (control.ThumbDepressed)
            {
                graphics.DrawElement("slider.horizontal.depressed", thumbBounds);
            }
            else if (control.MouseOverThumb)
            {
                graphics.DrawElement("slider.horizontal.highlighted", thumbBounds);
            }
            else {
                graphics.DrawElement("slider.horizontal.normal", thumbBounds);
            }

        }
    }
}
