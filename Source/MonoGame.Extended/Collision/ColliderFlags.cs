using System;

namespace MonoGame.Extended.Collision
{
    [Flags]
    internal enum ColliderFlags : byte
    {
        ShapeIsDirty = 1 << 0,
        WorldBoundingVolumeIsDirty = 1 << 1,
        All = ShapeIsDirty | WorldBoundingVolumeIsDirty
    }
}