using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Maps.Tiled
{
    public interface ITiledAnimated
    {
        TiledTilesetTile TilesetTile { get; }
        bool HasAnimation { get; }
        Vector2 Position { get; }
        int? CurrentTileId { get; }
        int? Gid { get; }
    }

    public class TiledTile : ITiledAnimated
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
        public Vector2 Position => new Vector2(X, Y);
        public bool IsBlank => Id == 0;
        public TiledTilesetTile TilesetTile { get; }

        public int? Gid => Id;
        public int? CurrentTileId => TilesetTile?.CurrentTileId + 1 ?? Id;
        public bool HasAnimation => TilesetTile != null && TilesetTile.Frames.Count != 0;

        public override string ToString()
        {
            return $"({X}, {Y}) - {Id}";
        }
    }
}