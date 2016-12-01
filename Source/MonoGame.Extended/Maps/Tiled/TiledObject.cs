using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject : ITiledAnimated
    {
        public TiledObject(TiledObjectType objectType, int id, int? gid, IShapeF shape, float x, float y, TiledTilesetTile tilesetTile = null)
            : this(objectType, id, gid, shape, new Vector2(x, y), tilesetTile)
        {
        }

        public TiledObject(TiledObjectType objectType, int id, int? gid, IShapeF shape, Vector2 position, TiledTilesetTile tilesetTile = null)
        {
            ObjectType = objectType;
            Id = id;
            Gid = gid;
            Shape = shape;
            Properties = new TiledProperties();
            Position = position;
            TilesetTile = tilesetTile;
        }

        public int Id { get; }
        public int? Gid { get; }
        public TiledObjectType ObjectType { get; }
        public string Name { get; set; }

        public Vector2 Position { get; }
        public TiledProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public string Type { get; set; }

        public IShapeF Shape { get; private set; }
        public float Width => Shape.BoundingRectangle.Width;
        public float Height => Shape.BoundingRectangle.Height;
        public SizeF Size => Shape.BoundingRectangle.Size;
        public RectangleF BoundingRectangle => Shape.BoundingRectangle;

        public TiledTilesetTile TilesetTile { get; }
        public bool HasAnimation => TilesetTile != null && TilesetTile.Frames.Count != 0;
        public int? CurrentTileId => TilesetTile?.CurrentTileId + 1 ?? Gid;

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public enum TiledObjectType
    {
        Rectangle = 1,
        Ellipse = 2,
        Polygon = 3,
        Polyline = 4,
        Tile = 5
    }
}