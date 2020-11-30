using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Extended.Tiled
{
    [DebuggerDisplay("{LocalTileIdentifier}: Type: {Type}, Properties: {Properties.Count}, Objects: {Objects.Count}")]
    public class TiledMapTilesetTile
    {
        public TiledMapTilesetTile(int localTileIdentifier, string type = null, TiledMapObject[] objects = null)
        {
            LocalTileIdentifier = localTileIdentifier;
            Type = type;
            Objects = objects != null ? new List<TiledMapObject>(objects) : new List<TiledMapObject>();
            Properties = new TiledMapProperties();
        }

        public int LocalTileIdentifier { get; }
        public string Type { get; }
        public TiledMapProperties Properties { get; }
        public List<TiledMapObject> Objects { get; }
    }
}