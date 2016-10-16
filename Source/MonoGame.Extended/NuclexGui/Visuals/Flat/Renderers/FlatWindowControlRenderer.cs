using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders window controls in a traditional flat style</summary>
    public class FlatWindowControlRenderer : IFlatControlRenderer<Controls.Desktop.GuiWindowControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(Controls.Desktop.GuiWindowControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();
            graphics.DrawElement("window", controlBounds);

            if (control.Title != null)
            {
                graphics.DrawString("window", controlBounds, control.Title);
            }
        }
    }
}
