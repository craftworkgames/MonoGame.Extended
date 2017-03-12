namespace MonoGame.Extended.NuclexGui.Visuals
{
    /// <summary>Interface for an exchangeable GUI painter</summary>
    public interface IGuiVisualizer
    {
        /// <summary>Renders an entire control tree starting at the provided control</summary>
        /// <param name="screen">Screen containing the GUI that will be drawn</param>
        void Draw(GuiScreen screen);
    }
}