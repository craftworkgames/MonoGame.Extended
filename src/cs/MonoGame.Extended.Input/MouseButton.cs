using System;

namespace MonoGame.Extended.Input
{
    [Flags]
    public enum MouseButton
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 4,
        XButton1 = 8,
        XButton2 = 16
    }
}