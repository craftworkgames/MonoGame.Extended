namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTilesetTileFrame
    {
        public TiledTilesetTileFrame(int order, int tileId, int duration)
        {
            Order = order;
            TileId = tileId;
            Duration = duration;
        }

        public int Order { get; set; }
        public int TileId { get; set; }
        public int Duration { get; set; }

        public override string ToString()
        {
            return $"{Order}:{TileId}:{Duration}";
        }
    }
}