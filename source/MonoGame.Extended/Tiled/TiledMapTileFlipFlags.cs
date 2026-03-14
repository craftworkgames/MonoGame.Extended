using System;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    [Flags]
    public enum TiledMapTileFlipFlags : uint
    {
        None = 0,
        FlipDiagonally = 0x20000000,
        FlipVertically = 0x40000000,
        FlipHorizontally = 0x80000000,
        All = FlipDiagonally | FlipVertically | FlipHorizontally
    }
}