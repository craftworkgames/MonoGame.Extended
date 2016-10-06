using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Collision.Shapes
{ 
    public abstract class CollisionShape2D : ConvexPolygon
    {
        protected internal CollisionShape2D(Transform2D transform = null) 
            : base(transform)
        {
        }

        protected internal CollisionShape2D(Vector2[] vertices, Transform2D transform = null)
            : base(vertices, transform)
        {
        }

        public Vector2 GetSupportPoint(Vector2 normal)
        {
            var vertices = WorldVertices;

            var bestProjection = float.MinValue;
            var supportPoint = default(Vector2);

            var verticesCount = vertices.Count;
            for (var i = 0; i < verticesCount; i++)
            {
                var vertex = vertices[i];
                var vectorProjection = vertex.Dot(normal);
                // ReSharper disable once InvertIf
                if (vectorProjection > bestProjection)
                {
                    bestProjection = vectorProjection;
                    supportPoint = vertex;
                }
            }

            return supportPoint;
        }
    }
}
