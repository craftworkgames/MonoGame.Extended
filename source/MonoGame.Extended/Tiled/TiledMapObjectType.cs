using System;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public enum TiledMapObjectType : byte
    {
        Rectangle,
        Ellipse,
        Polygon,
        Polyline,
        Tile
    }
}