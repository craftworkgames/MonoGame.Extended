namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetTile
    {
        internal TiledMapTilesetTile(int localTileIdentifier)
        {
            LocalTileIdentifier = localTileIdentifier;
            Properties = new TiledMapProperties();
        }

        public int LocalTileIdentifier { get; set; }
        public TiledMapProperties Properties { get; private set; }
    }
}