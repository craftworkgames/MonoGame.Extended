using MonoGame.Extended.NuclexGui.Controls;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders progress bars in a traditional flat style</summary>
    public class FlatProgressControlRenderer : IFlatControlRenderer<GuiProgressControl>
    {
        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiProgressControl control, IFlatGuiGraphics graphics)
        {
            var controlBounds = control.GetAbsoluteBounds();
            graphics.DrawElement("progress", controlBounds);

            controlBounds.Width *= control.Progress;
            graphics.DrawElement("progress.bar", controlBounds);
        }
    }
}