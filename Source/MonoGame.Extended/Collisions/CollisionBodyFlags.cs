using System;

namespace MonoGame.Extended.Collisions
{
    [Flags]
    public enum CollisionBodyFlags : byte
    {
        None = 0,
        IsBeingAdded = 1 << 0,
        IsBeingRemoved = 1 << 1,
        IsAwake = 1 << 2
    }
}
