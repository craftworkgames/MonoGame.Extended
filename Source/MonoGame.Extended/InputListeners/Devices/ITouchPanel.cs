using Microsoft.Xna.Framework;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Delegate used to report touch actions</summary>
    /// <param name="id">ID of the distinct touch</param>
    /// <param name="position">Position the action occurred at</param>
    public delegate void TouchDelegate(int id, Vector2 position);

    /// <summary>Specializd input devices for mouse-like controllers</summary>
    public interface ITouchPanel : IInputDevice
    {

        /// <summary>Triggered when the user presses on the screen</summary>
        event TouchDelegate Pressed;

        /// <summary>Triggered when the user moves his touch on the screen</summary>
        event TouchDelegate Moved;

        /// <summary>Triggered when the user releases the screen again</summary>
        event TouchDelegate Released;

        /// <summary>Maximum number of simultaneous touches the panel supports</summary>
        int MaximumTouchCount { get; }

        /// <summary>Retrieves the current state of the touch panel</summary>
        /// <returns>The current state of the touch panel</returns>
        TouchState GetState();
    }
}
