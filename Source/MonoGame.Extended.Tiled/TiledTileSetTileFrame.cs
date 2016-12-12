namespace MonoGame.Extended.Tiled
{
    public class TiledTilesetTileFrame
    {
        public int Order { get; set; }
        public int TileId { get; set; }
        public int Duration { get; set; }

        public TiledTilesetTileFrame(int order, int tileId, int duration)
        {
            Order = order;
            TileId = tileId;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"{Order}:{TileId}:{Duration}";
        }
    }
}