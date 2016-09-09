using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Collision.Shapes
{ 
    public abstract class CollisionShape2D : ConvexPolygon
    {
        private Vector2[] _normals;

        protected internal CollisionShape2D() 
            : base()
        {
        }

        protected internal CollisionShape2D(Vector2[] vertices)
            : base(vertices)
        {
        }

        protected void SetNormals(Vector2[] normals)
        {
            _normals = normals;
        }
    }
}
