using System.Collections.Generic;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetTile
    {
        public TiledMapTilesetTile(int localTileIdentifier, TiledMapObject[] objects = null)
        {
            LocalTileIdentifier = localTileIdentifier;
            Objects = objects != null ? new List<TiledMapObject>(objects) : new List<TiledMapObject>();
            Properties = new TiledMapProperties();
        }

        public int LocalTileIdentifier { get; }
        public TiledMapProperties Properties { get; }
        public List<TiledMapObject> Objects { get; }
    }
}