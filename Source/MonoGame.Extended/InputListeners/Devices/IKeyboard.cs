using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners.Devices
{
    /// <summary>Delegate used to report key presses and releases</summary>
    /// <param name="key">Key that has been pressed or released</param>
    public delegate void KeyDelegate(Keys key);

    /// <summary>Delegate used to report characters typed on a keyboard</summary>
    /// <param name="character">Character that has been typed</param>
    public delegate void CharacterDelegate(char character);

    /// <summary>Specialized input device for keyboard-like controllers</summary>
    public interface IKeyboard : IInputDevice
    {

        /// <summary>Fired when a key has been pressed</summary>
        event KeyDelegate KeyPressed;

        /// <summary>Fired when a key has been released</summary>
        event KeyDelegate KeyReleased;

        /// <summary>Fired when the user has entered a character</summary>
        /// <remarks>
        ///   This provides the complete, translated character the user has entered.
        ///   Handling of international keyboard layouts, shift key, accents and
        ///   other special cases is done by Windows according to the current users'
        ///   country and selected keyboard layout.
        /// </remarks>
        event CharacterDelegate CharacterEntered;

        /// <summary>Retrieves the current state of the keyboard</summary>
        /// <returns>The current state of the keyboard</returns>
        KeyboardState GetState();
    }
}
