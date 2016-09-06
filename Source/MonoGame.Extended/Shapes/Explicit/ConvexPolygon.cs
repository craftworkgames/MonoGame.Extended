using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public abstract class ConvexPolygon : Polygon
    {
        protected ConvexPolygon()
            : base()
        {
            
        }

        protected ConvexPolygon(Vector2[] vertices)
            : base(vertices)
        {
        }

        protected override void CalculateCentroid(out Vector2 centroid)
        {
            centroid.X = 0;
            centroid.Y = 0;
            var vertices = Points;
            var verticesCount = vertices.Count;

            for (var i = 0; i < verticesCount; i++)
            {
                centroid.X += vertices[i].X;
                centroid.Y += vertices[i].Y;
            }

            centroid /= verticesCount;
        }

    }
}
