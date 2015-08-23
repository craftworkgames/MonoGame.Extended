using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(TimeSpan time, MouseState previousState, MouseState currentState, 
            MouseButton button = MouseButton.None, int? scrollWheelValue = null, int? scrollWheelDelta = null)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Position = new Point(currentState.X, currentState.Y);
            Button = button;
            ScrollWheelValue = scrollWheelValue;
            ScrollWheelDelta = scrollWheelDelta;
            Time = time;
        }

        public TimeSpan Time { get; private set; }
        public MouseState PreviousState { get; private set; }
        public MouseState CurrentState { get; private set; }
        public MouseButton Button { get; private set; }
        public int? ScrollWheelValue { get; private set; }
        public int? ScrollWheelDelta { get; private set; }
        public Point Position { get; private set; }
    }
}
