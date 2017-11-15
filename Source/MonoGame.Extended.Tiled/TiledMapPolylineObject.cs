using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapPolylineObject : TiledMapObject
    {
        public TiledMapPolylineObject(int identifier, string name, Point2[] points, Size2 size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true) 
            : base(identifier, name, size, position, rotation, opacity, isVisible)
        {
            Points = points;
        }
        
        public Point2[] Points { get; }
        public override TiledMapObjectType Type => TiledMapObjectType.Polyline;
    }
}