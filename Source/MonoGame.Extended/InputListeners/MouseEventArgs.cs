using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(TimeSpan time, MouseState previousState, MouseState currentState, 
            MouseButton button = MouseButton.None, int? value = null, int? delta = null)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Position = new Point(currentState.X, currentState.Y);
            Button = button;
            Value = value;
            Delta = delta;
            Time = time;
        }

        public TimeSpan Time { get; private set; }
        public MouseState PreviousState { get; private set; }
        public MouseState CurrentState { get; private set; }
        public MouseButton Button { get; private set; }
        public int? Value { get; private set; }
        public int? Delta { get; private set; }
        public Point Position { get; private set; }
    }
}
