using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Delegate used to report movement of the mouse cursor</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    public delegate void MouseMoveDelegate(float x, float y);

    /// <summary>
    ///   Delegate used to report a press or released of one or more mouse buttons
    /// </summary>
    /// <param name="buttons">Button or buttons that have been pressed or released</param>
    public delegate void MouseButtonDelegate(MouseButton buttons);

    /// <summary>Delegate used to report a rotation of the mouse wheel</summary>
    /// <param name="ticks">Number of ticks the mouse wheel has been rotated</param>
    public delegate void MouseWheelDelegate(float ticks);

    /// <summary>Specializd input devices for mouse-like controllers</summary>
    public interface IMouse : IInputDevice
    {

        /// <summary>Fired when the mouse has been moved</summary>
        event MouseMoveDelegate MouseMoved;

        /// <summary>Fired when one or more mouse buttons have been pressed</summary>
        event MouseButtonDelegate MouseButtonPressed;

        /// <summary>Fired when one or more mouse buttons have been released</summary>
        event MouseButtonDelegate MouseButtonReleased;

        /// <summary>Fired when the mouse wheel has been rotated</summary>
        event MouseWheelDelegate MouseWheelRotated;

        /// <summary>Retrieves the current state of the mouse</summary>
        /// <returns>The current state of the mouse</returns>
        MouseState GetState();

        /// <summary>Moves the mouse cursor to the specified location</summary>
        /// <param name="x">New X coordinate of the mouse cursor</param>
        /// <param name="y">New Y coordinate of the mouse cursor</param>
        void MoveTo(float x, float y);
    }
}
