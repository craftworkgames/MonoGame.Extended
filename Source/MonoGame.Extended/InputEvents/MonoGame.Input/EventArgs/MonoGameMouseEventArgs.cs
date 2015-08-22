using System;

namespace Microsoft.Xna.Framework.Input
{
    internal class MonoGameMouseEventArgs : MouseEventArgs
    {
        internal TimeSpan Time { get; set; }

        internal MouseState Previous { get; private set; }
        internal MouseState Current { get; private set; }

        internal MonoGameMouseEventArgs(int x, int y, TimeSpan time, MouseState previous, MouseState current, 
            MouseButton button = MouseButton.None, int? value = null, int? delta = null)
            : base(x, y, button, value, delta)
        {
            X = x;
            Y = y;
            Time = time;
            Previous = previous;
            Current = current;
        }
    }
}
