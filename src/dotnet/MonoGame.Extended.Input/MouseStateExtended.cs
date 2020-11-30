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
        public Point Position => _currentMouseState.Position;
        public bool PositionChanged => _currentMouseState.Position != _previousMouseState.Position;

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

        private bool IsPressed(Func<MouseState, ButtonState> button) => button(_currentMouseState) == ButtonState.Pressed;
        private bool IsReleased(Func<MouseState, ButtonState> button) => button(_currentMouseState) == ButtonState.Released;
        private bool WasJustPressed(Func<MouseState, ButtonState> button) => button(_previousMouseState) == ButtonState.Released && button(_currentMouseState) == ButtonState.Pressed;
        private bool WasJustReleased(Func<MouseState, ButtonState> button) => button(_previousMouseState) == ButtonState.Pressed && button(_currentMouseState) == ButtonState.Released;
    }
}