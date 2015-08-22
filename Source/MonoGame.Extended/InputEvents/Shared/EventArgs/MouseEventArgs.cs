using System;

namespace Microsoft.Xna.Framework.Input
{
    public abstract class MouseEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MouseButton Button { get; protected set; }

        public int? Value { get; protected set; }
        public int? Delta { get; protected set; }

        protected internal MouseEventArgs(int x, int y, MouseButton button, int? value, int? delta) 
        {
            X = x;
            Y = y;
            Button = button;
            Value = value;
            Delta = delta;
        }
    }
}
