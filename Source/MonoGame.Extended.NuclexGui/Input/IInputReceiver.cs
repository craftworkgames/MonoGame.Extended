using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.NuclexGui.Input
{
    /// <summary>Interface for classes that can process user input</summary>
    /// <remarks>
    ///     This interface is implemented by any class that can process user input.
    ///     Normally, user input is directly fed into the <see cref="GuiScreen" /> class
    ///     which manages the global state of an isolated GUI system. It is also possible,
    ///     though not recommended, to use this interface for sending input directly
    ///     to a control, for example, to simulate text input for an input box.
    /// </remarks>
    public interface IInputReceiver
    {
        /// <summary>Injects an input command into the input receiver</summary>
        /// <param name="command">Input command to be injected</param>
        /// <remarks>
        ///     <para>
        ///         If the GUI is run without the usual GUI input methods (eg. when a GUI is
        ///         displayed on a gaming console), this is the sole way to feed input to
        ///         the controls.
        ///     </para>
        ///     <para>
        ///         By default, normal key presses will generate a command in addition to the
        ///         KeyPress itself, so unless a control does something very special, it
        ///         should respond to this method only and leave the KeyPress method alone ;)
        ///     </para>
        /// </remarks>
        void InjectCommand(Command command);

        /// <summary>Called when a button on the gamepad has been pressed</summary>
        /// <param name="button">Button that has been pressed</param>
        void InjectButtonPress(Buttons button);

        /// <summary>Called when a button on the gamepad has been released</summary>
        /// <param name="button">Button that has been released</param>
        void InjectButtonRelease(Buttons button);

        /// <summary>Injects a mouse position update into the receiver</summary>
        /// <param name="x">New X coordinate of the mouse cursor on the screen</param>
        /// <param name="y">New Y coordinate of the mouse cursor on the screen</param>
        /// <remarks>
        ///     When the mouse leaves the valid region (eg. if the game runs in windowed mode
        ///     and the mouse cursor is moved outside of the window), a final mouse move
        ///     notification is generated with the coordinates -1, -1
        /// </remarks>
        void InjectMouseMove(float x, float y);

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        void InjectMousePress(MouseButton button);

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        void InjectMouseRelease(MouseButton button);

        /// <summary>Called when the mouse wheel has been rotated</summary>
        /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
        void InjectMouseWheel(float ticks);

        /// <summary>Called when a key on the keyboard has been pressed down</summary>
        /// <param name="keyCode">Code of the key that was pressed</param>
        /// <remarks>
        ///     Only handle this if you need it for some special purpose. For standard commands
        ///     like confirmation and cancellation, simply respond to InjectCommand()
        /// </remarks>
        void InjectKeyPress(Keys keyCode);

        /// <summary>Called when a key on the keyboard has been released again</summary>
        /// <param name="keyCode">Code of the key that was released</param>
        /// <remarks>
        ///     Only handle this if you need it for some special purpose. For standard commands
        ///     like confirmation and cancellation, simply respond to InjectCommand()
        /// </remarks>
        void InjectKeyRelease(Keys keyCode);

        /// <summary>Handle user text input by a physical or virtual keyboard</summary>
        /// <param name="character">Character that has been entered</param>
        void InjectCharacter(char character);
    }
}