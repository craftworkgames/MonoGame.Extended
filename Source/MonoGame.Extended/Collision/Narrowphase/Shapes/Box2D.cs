using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Narrowphase.Shapes
{
    public sealed class Box2D : CollisionShape2D
    {
        private SizeF _size;

        public SizeF Size
        {
            get { return _size; }
            set
            {
                _size = value * 0.5f;
                Invalidate();
            }
        }

        public Box2D()
        {
            SetPoints(new Vector2[4]);

            var normals = new Vector2[4];
            // counter clockwise
            normals[0] = new Vector2(0, 1); // top
            normals[3] = new Vector2(1, 0); // right
            normals[2] = new Vector2(0, -1); // bottom
            normals[1] = new Vector2(-1, 0); // left
            SetNormals(normals);
        }

        protected override void CalculateCentroid(out Vector2 centroid)
        {
            centroid.X = _size.Width * 0.5f;
            centroid.Y = _size.Height * 0.5f;
        }

        protected override void UpdateVertices(Vector2[] vertices)
        {
            // counter clockwise
            vertices[0] = new Vector2(-_size.Width, _size.Height); // top-left
            vertices[3] = new Vector2(_size.Width, _size.Height); // top-right
            vertices[2] = new Vector2(_size.Width, -_size.Height); // bottom-right
            vertices[1] = new Vector2(-_size.Width, -_size.Height); // bottom-left
        }
    }
}
