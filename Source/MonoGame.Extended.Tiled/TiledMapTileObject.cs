using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapTileObject : TiledMapObject
    {
        public TiledMapTilesetTile TilesetTile { get; }

        public TiledMapTileset Tileset { get; }

        public TiledMapTileObject(ContentReader input, TiledMap map)
            : base(input)
        {
            var globalTileIdentifierWithFlags = input.ReadUInt32();
            var tile = new TiledMapTile(globalTileIdentifierWithFlags);
            Tileset = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);

            var localTileIdentifier = tile.GlobalIdentifier - Tileset.FirstGlobalIdentifier;
            TilesetTile = Tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
        }
    }
}
