using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Shapes
{
    public sealed class Box2D : CollisionShape2D
    {
        private SizeF _size;

        public SizeF Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                Invalidate();
            }
        }

        public Box2D(Transform2D transform = null)
            : base(new Vector2[4], transform)
        {
        }

        protected override void CalculateLocalCentroid(out Vector2 centroid)
        {
            centroid = default(Vector2);
        }

        protected override void UpdateLocalVertices(Vector2[] localVertices)
        {
            // counter clockwise

            var halfSize = _size * 0.5f;
            localVertices[0] = new Vector2(-halfSize.Width, halfSize.Height); // top-left
            localVertices[3] = new Vector2(halfSize.Width, halfSize.Height); // top-right
            localVertices[2] = new Vector2(halfSize.Width, -halfSize.Height); // bottom-right
            localVertices[1] = new Vector2(-halfSize.Width, -halfSize.Height); // bottom-left
        }
    }
}
