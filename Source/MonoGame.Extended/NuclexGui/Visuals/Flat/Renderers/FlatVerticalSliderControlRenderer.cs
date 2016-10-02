using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders sliders in a traditional flat style</summary>
    public class FlatVerticalSliderControlRenderer : IFlatControlRenderer<Controls.Desktop.GuiVerticalSliderControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(
          Controls.Desktop.GuiVerticalSliderControl control, IFlatGuiGraphics graphics
        )
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            float thumbHeight = controlBounds.Height * control.ThumbSize;
            float thumbY = (controlBounds.Height - thumbHeight) * control.ThumbPosition;

            graphics.DrawElement("rail.vertical", controlBounds);

            RectangleF thumbBounds = new RectangleF(
              controlBounds.X, controlBounds.Y + thumbY, controlBounds.Width, thumbHeight
            );

            if (control.ThumbDepressed)
            {
                graphics.DrawElement("slider.vertical.depressed", thumbBounds);
            }
            else if (control.MouseOverThumb)
            {
                graphics.DrawElement("slider.vertical.highlighted", thumbBounds);
            }
            else {
                graphics.DrawElement("slider.vertical.normal", thumbBounds);
            }

        }
    }
}
