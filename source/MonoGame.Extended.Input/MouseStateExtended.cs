using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Input
{
    public struct MouseStateExtended
    {
        private readonly MouseState _currentMouseState;
        private readonly MouseState _previousMouseState;

        public MouseStateExtended(MouseState currentMouseState, MouseState previousMouseState)
        {
            _currentMouseState = currentMouseState;
            _previousMouseState = previousMouseState;
        }

        public int X => _currentMouseState.X;
        public int Y => _currentMouseState.Y;
        public Point Position => new Point(_currentMouseState.X, _currentMouseState.Y);
        public bool PositionChanged => _currentMouseState.X != _previousMouseState.X || _currentMouseState.Y != _previousMouseState.Y;

        public int DeltaX => _previousMouseState.X - _currentMouseState.X;
        public int DeltaY => _previousMouseState.Y - _currentMouseState.Y;
        public Point DeltaPosition => new Point(DeltaX, DeltaY);

        public int ScrollWheelValue => _currentMouseState.ScrollWheelValue;
        public int DeltaScrollWheelValue => _previousMouseState.ScrollWheelValue - _currentMouseState.ScrollWheelValue;

        public ButtonState LeftButton => _currentMouseState.LeftButton;
        public ButtonState MiddleButton => _currentMouseState.MiddleButton;
        public ButtonState RightButton => _currentMouseState.RightButton;
        public ButtonState XButton1 => _currentMouseState.XButton1;
        public ButtonState XButton2 => _currentMouseState.XButton2;

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
        /// Get the just-down state for the mouse on this state-change: true if the mouse button has just been pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <remarks>Deprecated because of inconsistency with <see cref="KeyboardStateExtended"/></remarks>
        /// <returns>The just-down state for the mouse on this state-change.</returns>
        [Obsolete($"Deprecated in favor of {nameof(IsButtonPressed)}")]
        public bool WasButtonJustDown(MouseButton button)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (button)
            {
                case MouseButton.Left: return WasJustPressed(m => m.LeftButton);
                case MouseButton.Middle: return WasJustPressed(m => m.MiddleButton);
                case MouseButton.Right: return WasJustPressed(m => m.RightButton);
                case MouseButton.XButton1: return WasJustPressed(m => m.XButton1);
                case MouseButton.XButton2: return WasJustPressed(m => m.XButton2);
            }

            return false;
        }

        /// <summary>
        /// Get the just-up state for the mouse on this state-change: true if the mouse button has just been released.
        /// </summary>
        /// <param name="button"></param>
        /// <remarks>Deprecated because of inconsistency with <see cref="KeyboardStateExtended"/></remarks>
        /// <returns>The just-up state for the mouse on this state-change.</returns>
        [Obsolete($"Deprecated in favor of {nameof(IsButtonReleased)}")]
        public bool WasButtonJustUp(MouseButton button)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (button)
            {
                case MouseButton.Left: return WasJustReleased(m => m.LeftButton);
                case MouseButton.Middle: return WasJustReleased(m => m.MiddleButton);
                case MouseButton.Right: return WasJustReleased(m => m.RightButton);
                case MouseButton.XButton1: return WasJustReleased(m => m.XButton1);
                case MouseButton.XButton2: return WasJustReleased(m => m.XButton2);
            }

            return false;
        }

        /// <summary>
        /// Get the pressed state of a mouse button, for this state-change.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>true if the given mouse button was pressed this state-change, otherwise false</returns>
        public readonly bool IsButtonPressed(MouseButton button) => button switch
        {
            MouseButton.Left => WasJustPressed(m => m.LeftButton),
            MouseButton.Middle => WasJustPressed(m => m.MiddleButton),
            MouseButton.Right => WasJustPressed(m => m.RightButton),
            MouseButton.XButton1 => WasJustPressed(m => m.XButton1),
            MouseButton.XButton2 => WasJustPressed(m => m.XButton2),
            _ => false,
        };

        /// <summary>
        /// Get the released state of a mouse button, for this state-change.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>true if the given mouse button was released this state-change, otherwise false</returns>
        public readonly bool IsButtonReleased(MouseButton button) => button switch
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
}
