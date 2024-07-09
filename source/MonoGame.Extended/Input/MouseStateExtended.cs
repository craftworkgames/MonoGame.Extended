using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input;

/// <summary>
/// Represents the state of mouse input.
/// </summary>
/// <remarks>
/// This is an extended version of the base <see cref="Microsoft.Xna.Framework.Input.MouseState"/> struct
/// that provides utility for checking state differences between the previous and current state.
/// </remarks>
public struct MouseStateExtended
{
    private readonly MouseState _currentMouseState;
    private readonly MouseState _previousMouseState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseStateExtended"/> value.
    /// </summary>
    /// <param name="currentMouseState">The state of mouse input during the current update cycle.</param>
    /// <param name="previousMouseState">THe state of mouse input during hte previous update cycle.</param>
    public MouseStateExtended(MouseState currentMouseState, MouseState previousMouseState)
    {
        _currentMouseState = currentMouseState;
        _previousMouseState = previousMouseState;
    }

    /// <summary>
    /// Gets the current x-coordinate position of the mouse cursor relative to the game window.
    /// </summary>
    public int X => _currentMouseState.X;

    /// <summary>
    /// Gets the current y-coordinate position of the mouse cursor relative to the game window.
    /// </summary>
    public int Y => _currentMouseState.Y;

    /// <summary>
    /// Gets the current xy-coordinate position of the mouse cursor relative to the game window.
    /// </summary>
    public Point Position => new Point(_currentMouseState.X, _currentMouseState.Y);

    /// <summary>
    /// Gets a value that indicates whether the position of the mouse cursor changes between the previous and current
    /// states.
    /// </summary>
    public bool PositionChanged => _currentMouseState.X != _previousMouseState.X || _currentMouseState.Y != _previousMouseState.Y;

    /// <summary>
    /// Gets the difference in the x-coordinate position change of the mouse between the previous and current state.
    /// </summary>
    public int DeltaX => _previousMouseState.X - _currentMouseState.X;

    /// <summary>
    /// Gets the difference in the y-coordinate position change of the mouse between the previous and current state.
    /// </summary>
    public int DeltaY => _previousMouseState.Y - _currentMouseState.Y;

    /// <summary>
    /// Gets the difference in the xy-coordinate position change of the mouse between the previous and curren state.
    /// </summary>
    public Point DeltaPosition => new Point(DeltaX, DeltaY);

    /// <summary>
    /// Gets the current value of the mouse scroll wheel.
    /// </summary>
    public int ScrollWheelValue => _currentMouseState.ScrollWheelValue;

    /// <summary>
    /// Gets the difference in the mouse scroll wheel value between the previous and current state.
    /// </summary>
    public int DeltaScrollWheelValue => _previousMouseState.ScrollWheelValue - _currentMouseState.ScrollWheelValue;

    /// <summary>
    /// Gets the current state of the mouse left button.
    /// </summary>
    public ButtonState LeftButton => _currentMouseState.LeftButton;

    /// <summary>
    /// Gets the current state of the mouse middle button.
    /// </summary>
    public ButtonState MiddleButton => _currentMouseState.MiddleButton;

    /// <summary>
    /// Gets the current state of the mouse right button.
    /// </summary>
    public ButtonState RightButton => _currentMouseState.RightButton;

    /// <summary>
    /// Gets the current state of the first mouse extra button.
    /// </summary>
    public ButtonState XButton1 => _currentMouseState.XButton1;

    /// <summary>
    /// Gets the current state of the second mouse extra button.
    /// </summary>
    public ButtonState XButton2 => _currentMouseState.XButton2;

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button is down during the current state.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>
    /// <see langword="true"/> if the mouse button is down during the current state; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsButtonDown(MouseButton button)
    {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (button)
        {
            case MouseButton.Left:     return IsPressed(m => m.LeftButton);
            case MouseButton.Middle:   return IsPressed(m => m.MiddleButton);
            case MouseButton.Right:    return IsPressed(m => m.RightButton);
            case MouseButton.XButton1: return IsPressed(m => m.XButton1);
            case MouseButton.XButton2: return IsPressed(m => m.XButton2);
        }

        return false;
    }

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button is up during the current state.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>
    /// <see langword="true"/> if the mouse button is up during the current state; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsButtonUp(MouseButton button)
    {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (button)
        {
            case MouseButton.Left:     return IsReleased(m => m.LeftButton);
            case MouseButton.Middle:   return IsReleased(m => m.MiddleButton);
            case MouseButton.Right:    return IsReleased(m => m.RightButton);
            case MouseButton.XButton1: return IsReleased(m => m.XButton1);
            case MouseButton.XButton2: return IsReleased(m => m.XButton2);
        }

        return false;
    }

    /// <summary>
    /// Returns whether the specified mouse button was up during the previous, but is now down.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>
    /// <see langword="true"/> if the mouse button was up pressed this state-change; otherwise, <see langword="false"/>.
    /// </returns>
    public readonly bool WasButtonPressed(MouseButton button) => button switch
    {
        MouseButton.Left => WasJustPressed(m => m.LeftButton),
        MouseButton.Middle => WasJustPressed(m => m.MiddleButton),
        MouseButton.Right => WasJustPressed(m => m.RightButton),
        MouseButton.XButton1 => WasJustPressed(m => m.XButton1),
        MouseButton.XButton2 => WasJustPressed(m => m.XButton2),
        _ => false,
    };

    /// <summary>
    /// Returns whether the specified mouse button was down during the previous state, but is now up.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>
    /// <see langword="true"/> if the mouse button was released this state-change; otherwise, <see langword="false"/>.
    /// </returns>
    public readonly bool WasButtonReleased(MouseButton button) => button switch
    {
        MouseButton.Left => WasJustReleased(m => m.LeftButton),
        MouseButton.Middle => WasJustReleased(m => m.MiddleButton),
        MouseButton.Right => WasJustReleased(m => m.RightButton),
        MouseButton.XButton1 => WasJustReleased(m => m.XButton1),
        MouseButton.XButton2 => WasJustReleased(m => m.XButton2),
        _ => false,
    };

    private readonly bool IsPressed(Func<MouseState, ButtonState> button)
        => button(_currentMouseState) == ButtonState.Pressed;
    private readonly bool IsReleased(Func<MouseState, ButtonState> button)
        => button(_currentMouseState) == ButtonState.Released;
    private readonly bool WasJustPressed(Func<MouseState, ButtonState> button)
        => button(_previousMouseState) == ButtonState.Released && button(_currentMouseState) == ButtonState.Pressed;
    private readonly bool WasJustReleased(Func<MouseState, ButtonState> button)
        => button(_previousMouseState) == ButtonState.Pressed && button(_currentMouseState) == ButtonState.Released;
}
