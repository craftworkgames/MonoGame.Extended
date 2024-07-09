using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input;

/// <summary>
/// Represents keyboard input.
/// </summary>
/// <remarks>
/// This is an extended version of the default <see cref="Microsoft.Xna.Framework.Input.Keyboard"/> class
/// which offers internal tracking of both the previous and current state of keyboard input.
/// </remarks>
public static class KeyboardExtended
{
    private static KeyboardState _currentKeyboardState;
    private static KeyboardState _previousKeyboardState;

    /// <summary>
    /// Gets the state of keyboard input.
    /// </summary>
    /// <returns>
    /// A <see cref="KeyboardStateExtended"/> value that represents the state of keyboard input.
    /// </returns>
    public static KeyboardStateExtended GetState()
    {
        return new KeyboardStateExtended(_currentKeyboardState, _previousKeyboardState);
    }

    /// <summary>
    /// Updates the <see cref="KeyboardExtended"/>
    /// </summary>
    /// <remarks>
    /// This internally will overwrite the source data for the previous state with the current state, then get the
    /// current state from the keyboard input.  This should only be called once per update cycle.  Calling it more
    /// than once per update cycle can result in the cached previous state being overwritten with invalid data.
    /// </remarks>
    public static void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }
}
