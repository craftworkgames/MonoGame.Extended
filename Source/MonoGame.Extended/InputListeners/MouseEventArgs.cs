using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.InputListeners
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(int x, int y, TimeSpan time, MouseState previous, MouseState current, 
            MouseButton button = MouseButton.None, int? value = null, int? delta = null)
        {
            X = x;
            Y = y;
            Button = button;
            Value = value;
            Delta = delta;
            Time = time;
            Previous = previous;
            Current = current;
        }

        public TimeSpan Time { get; set; }
        public MouseState Previous { get; private set; }
        public MouseState Current { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public MouseButton Button { get; protected set; }
        public int? Value { get; protected set; }
        public int? Delta { get; protected set; }
    }
}
