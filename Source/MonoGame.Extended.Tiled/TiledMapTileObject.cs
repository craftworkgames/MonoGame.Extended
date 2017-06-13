using System.Linq;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapTileObject : TiledMapObject
    {
        public TiledMapTilesetTile TilesetTile { get; }

        public TiledMapTileObject(ContentReader input, TiledMap map)
            : base(input)
        {
            var globalTileIdentifierWithFlags = input.ReadUInt32();
            var tile = new TiledMapTile(globalTileIdentifierWithFlags);
            var tileset = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
            var localTileIdentifier = tile.GlobalIdentifier - tileset.FirstGlobalIdentifier;
            TilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
        }
    }
}
