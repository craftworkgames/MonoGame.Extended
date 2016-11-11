using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject : ITiledAnimated
    {
        public TiledObject(TiledObjectType objectType, int id, int? gid, float x, float y, float width, float height, TiledTilesetTile tilesetTile = null)
            : this(objectType, id, gid, new Vector2(x, y), width, height, tilesetTile)
        {
        }

        public TiledObject(TiledObjectType objectType, int id, int? gid, Vector2 position, float width, float height, TiledTilesetTile tilesetTile = null)
        {
            ObjectType = objectType;
            Id = id;
            Gid = gid;
            Width = width;
            Height = height;
            Points = new List<Vector2>();
            Properties = new TiledProperties();
            Position = position;
            TilesetTile = tilesetTile;
        }

        public int Id { get; }
        public int? Gid { get; }
        public TiledObjectType ObjectType { get; }
        public string Name { get; set; }
        public float Width { get; }
        public float Height { get; }
        public Vector2 Position { get; }
        public SizeF Size => new SizeF(Width, Height);
        public RectangleF BoundingRectangle => new RectangleF(Position, Size);
        public TiledProperties Properties { get; }
        public List<Vector2> Points { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public string Type { get; set; }

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