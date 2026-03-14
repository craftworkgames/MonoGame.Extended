using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    [Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
    public sealed class TiledMapTileObject : TiledMapObject
    {
        public TiledMapTileObject(int identifier, string name, TiledMapTileset tileset, TiledMapTilesetTile tile,
            SizeF size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true, string type = null)
            : base(identifier, name, size, position, rotation, opacity, isVisible, type)
        {
            Tileset = tileset;
            Tile = tile;
        }

        public TiledMapTilesetTile Tile { get; }
        public TiledMapTileset Tileset { get; }
    }
}
