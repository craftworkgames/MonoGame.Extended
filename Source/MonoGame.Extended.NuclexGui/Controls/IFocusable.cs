namespace MonoGame.Extended.NuclexGui.Controls
{
    /// <summary>Interface for controls which can obtain the input focus</summary>
    /// <remarks>
    ///     Implement this interface in any control which can obtain the input focus.
    /// </remarks>
    public interface IFocusable
    {
        /// <summary>Whether the control can currently obtain the input focus</summary>
        /// <remarks>
        ///     Usually returns true. For controls that can be disabled to disallow user
        ///     interaction, false can be returned to prevent the control from being
        ///     traversed when the user presses the tab key or uses the cursor / game pad
        ///     to select a control.
        /// </remarks>
        bool CanGetFocus { get; }
    }
}