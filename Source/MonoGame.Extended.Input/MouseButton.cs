using System;

namespace MonoGame.Extended.Input
{
    [Flags]
    public enum MouseButton
    {
        None = 0,
        Left = 1048576,
        Middle = 4194304,
        Right = 2097152,
        XButton1 = 8388608,
        XButton2 = 16777216
    }
}