using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders sliders in a traditional flat style</summary>
    public class FlatVerticalSliderControlRenderer : IFlatControlRenderer<GuiVerticalSliderControl>
    {
        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(
            GuiVerticalSliderControl control, IFlatGuiGraphics graphics
        )
        {
            var controlBounds = control.GetAbsoluteBounds();

            var thumbHeight = controlBounds.Height*control.ThumbSize;
            var thumbY = (controlBounds.Height - thumbHeight)*control.ThumbPosition;

            graphics.DrawElement("rail.vertical", controlBounds);

            var thumbBounds = new RectangleF(
                controlBounds.X, controlBounds.Y + thumbY, controlBounds.Width, thumbHeight
            );

            if (control.ThumbDepressed)
                graphics.DrawElement("slider.vertical.depressed", thumbBounds);
            else if (control.MouseOverThumb)
                graphics.DrawElement("slider.vertical.highlighted", thumbBounds);
            else graphics.DrawElement("slider.vertical.normal", thumbBounds);
        }
    }
}