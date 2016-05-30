using System;

namespace MonoGame.Extended.Collisions
{
    [Flags]
    public enum CollisionFixtureFlags
    {
        None = 0,
        IsBeingAdded = 1 << 0,
        IsBeingRemoved = 1 << 1,
    }
}
