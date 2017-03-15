
namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>
    ///     Interface which can be established between a control and its renderer to
    ///     allow a list control to locate the list row the cursor is in
    /// </summary>
    /// <remarks>
    ///     A renderer can implement this interface and assign it to a control that
    ///     it renders so the control can ask the renderer for extended informations
    ///     regarding the look of its text. If this interface is provided, certain
    ///     controls will be able to correctly place the caret in user-editable text
    ///     when they are clicked by the mouse.
    /// </remarks>
    public interface IListRowLocator
    {
        /// <summary>Calculates the list row the cursor is in</summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <param name="thumbPosition">
        ///     Position of the thumb in the list's slider
        /// </param>
        /// <param name="itemCount">
        ///     Number of items contained in the list
        /// </param>
        /// <param name="y">Vertical position of the cursor</param>
        /// <returns>The row the cursor is over</returns>
        int GetRow(RectangleF bounds, float thumbPosition, int itemCount, float y);

        /// <summary>Determines the height of a row displayed in the list</summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <returns>The height of a single row in the list</returns>
        float GetRowHeight(RectangleF bounds);
    }
}