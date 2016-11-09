using MonoGame.Extended.NuclexGui.Controls;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    /// <summary>Interface for a class that renders a control</summary>
    public interface IFlatControlRenderer
    {
    }

    /// <summary>Interface for a class responsible to render a specific control type</summary>
    /// <typeparam name="ControlType">Type of control the implementation class will render</typeparam>
    public interface IFlatControlRenderer<TControlType> : IFlatControlRenderer where TControlType : GuiControl
    {
        /// <summary>Renders the specified control using the provided graphics interface</summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">Graphics interface that will be used to draw the control</param>
        void Render(TControlType control, IFlatGuiGraphics graphics);
    }
}