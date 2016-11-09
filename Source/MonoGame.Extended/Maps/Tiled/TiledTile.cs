namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTile
    {
        public TiledTile(int id, int x, int y, TiledTilesetTile tilesetTile = null)
        {
            Id = id;
            X = x;
            Y = y;
            TilesetTile = tilesetTile;
        }

        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public bool IsBlank => Id == 0;
        public TiledTilesetTile TilesetTile { get; set; }

        public int CurrentTileId => (TilesetTile == null) || !TilesetTile.CurrentTileId.HasValue
            ? Id
            : TilesetTile.CurrentTileId.Value + 1;

        public bool HasAnimation => (TilesetTile == null) || (TilesetTile.Animation.Count == 0) ? false : true;

        public override string ToString()
        {
            return $"({X}, {Y}) - {Id}";
        }
    }
}