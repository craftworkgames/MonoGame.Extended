using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Collision.Shapes
{ 
    public abstract class CollisionShape2D : ConvexPolygon
    {
        private Vector2[] _normals;

        public IReadOnlyList<Vector2> Normals => _normals;

        protected internal CollisionShape2D() 
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
