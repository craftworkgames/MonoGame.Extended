
namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>
    ///     Interface which can be established between a control and its renderer to
    ///     allow a slider control to locate its thumb
    /// </summary>
    /// <remarks>
    ///     A renderer can implement this interface and assign it to a control that
    ///     it renders so the control can ask the renderer for extended informations
    ///     regarding the look of its text. If this interface is provided, certain
    ///     controls will be able to correctly place the caret in user-editable text
    ///     when they are clicked by the mouse.
    /// </remarks>
    public interface IThumbLocator
    {
        /// <summary>
        ///     Calculates the position of the thumb on a slider
        /// </summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <param name="thumbPosition">Relative position of the thumb (0.0 .. 1.0)</param>
        /// <param name="thumbSize">Relative size of the thumb (0.0 .. 1.0)</param>
        /// <returns>The region covered by the slider's thumb</returns>
        RectangleF GetThumbPosition(RectangleF bounds, float thumbPosition, float thumbSize);
    }
}