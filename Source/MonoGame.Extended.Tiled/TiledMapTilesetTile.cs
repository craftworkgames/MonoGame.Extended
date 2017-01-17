namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetTile
    {
        public int LocalTileIdentifier { get; set; }
        public TiledMapProperties Properties { get; private set; }

        internal TiledMapTilesetTile(int localTileIdentifier)
        {
            LocalTileIdentifier = localTileIdentifier;
            Properties = new TiledMapProperties();
        }
    }
}