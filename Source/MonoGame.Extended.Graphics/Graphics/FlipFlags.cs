using System;

namespace MonoGame.Extended.Graphics
{
    [Flags]
    public enum FlipFlags : byte
    {
        None = 0,
        FlipDiagonally = 1 << 0,
        FlipVertically = 1 << 1,
        FlipHorizontally = 1 << 2
    }
}
