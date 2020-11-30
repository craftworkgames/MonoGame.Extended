using MonoGame.Extended.NuclexGui.Visuals;

namespace MonoGame.Extended.NuclexGui
{
    /// <summary>Game-wide interface for the GUI manager component</summary>
    public interface IGuiService
    {
        /// <summary>GUI that is being rendered</summary>
        /// <remarks>
        ///     The GUI manager renders one GUI full-screen onto the primary render target
        ///     (the backbuffer). This property holds the GUI that is being managed by
        ///     the GUI manager component. You can replace it at any time, for example,
        ///     if the player opens or closes your ingame menu.
        /// </remarks>
        GuiScreen Screen { get; set; }

        /// <summary>
        ///     Responsible for creating a visual representation of the GUI on the screen
        /// </summary>
        IGuiVisualizer Visualizer { get; set; }
    }
}