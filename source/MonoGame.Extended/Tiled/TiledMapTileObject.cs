using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
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
