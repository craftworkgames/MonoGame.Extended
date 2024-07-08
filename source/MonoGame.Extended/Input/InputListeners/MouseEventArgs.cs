using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Input.InputListeners
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(ViewportAdapter viewportAdapter, TimeSpan time, MouseState previousState,
            MouseState currentState,
            MouseButton button = MouseButton.None)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Position = viewportAdapter?.PointToScreen(currentState.X, currentState.Y)
                       ?? new Point(currentState.X, currentState.Y);
            Button = button;
            ScrollWheelValue = currentState.ScrollWheelValue;
            ScrollWheelDelta = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
            Time = time;
        }

        public TimeSpan Time { get; }

        public MouseState PreviousState { get; }
        public MouseState CurrentState { get; }
        public Point Position { get; }
        public MouseButton Button { get; }
        public int ScrollWheelValue { get; }
        public int ScrollWheelDelta { get; }

        public Vector2 DistanceMoved => new Vector2(CurrentState.X, CurrentState.Y) - new Vector2(PreviousState.X, PreviousState.Y);
    }
}