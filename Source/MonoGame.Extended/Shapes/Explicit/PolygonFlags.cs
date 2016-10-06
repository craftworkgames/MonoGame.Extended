using System;

namespace MonoGame.Extended.Shapes.Explicit
{
    [Flags]
    internal enum PolygonFlags : byte
    {
        LocalVerticesAreDirty = 1 << 0,
        WorldVerticesAndNormalsAreDirty = 1 << 1,
        All = LocalVerticesAreDirty | WorldVerticesAndNormalsAreDirty
    }
}