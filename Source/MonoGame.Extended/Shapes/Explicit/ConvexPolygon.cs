using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public abstract class ConvexPolygon : Polygon
    {
        protected ConvexPolygon(Transform2D transform = null)
            : base(transform)
        {
            
        }

        protected ConvexPolygon(Vector2[] vertices, Transform2D transform = null)
            : base(vertices, transform)
        {
        }

        protected override void CalculateLocalCentroid(out Vector2 centroid)
        {
            centroid.X = 0;
            centroid.Y = 0;
            var vertices = LocalVertices;
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
