using MonoGame.Extended.NuclexGui.Controls;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders label controls in a traditional flat style</summary>
    public class FlatLabelControlRenderer : IFlatControlRenderer<GuiLabelControl>
    {
        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiLabelControl control, IFlatGuiGraphics graphics)
        {
            graphics.DrawString("label", control.GetAbsoluteBounds(), control.Text);
        }
    }
}