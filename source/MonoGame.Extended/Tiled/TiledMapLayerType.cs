using System;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public enum TiledMapLayerType : byte
    {
        ImageLayer = 0,
        TileLayer = 1,
        ObjectLayer = 2,
		GroupLayer = 3
    }
}