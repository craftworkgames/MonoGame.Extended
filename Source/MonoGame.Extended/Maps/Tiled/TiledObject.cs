using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject
    {
        public int? Gid { get; }
        public float Height { get; }

        public int Id { get; }
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public TiledObjectType ObjectType { get; }
        public float Opacity { get; set; }
        public List<Vector2> Points { get; }
        public TiledProperties Properties { get; }
        public float Rotation { get; set; }
        public string Type { get; set; }
        public float Width { get; }
        public float X { get; }
        public float Y { get; }

        public TiledObject(TiledObjectType objectType, int id, int? gid, float x, float y, float width, float height)
        {
            ObjectType = objectType;
            Id = id;
            Gid = gid;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Points = new List<Vector2>();
            Properties = new TiledProperties();
        }

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