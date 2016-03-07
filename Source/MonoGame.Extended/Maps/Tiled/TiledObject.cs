using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject
    {
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

        public int Id { get; }
        public int? Gid { get; }
        public TiledObjectType ObjectType { get; }
        public string Name { get; set; }
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public TiledProperties Properties { get; }
        public List<Vector2> Points { get; } 
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public string Type { get; set; }


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
