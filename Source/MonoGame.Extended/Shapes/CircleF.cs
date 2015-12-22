using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class CircleF : ShapeF<CircleF>
    {
        public CircleF(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector2 Center { get; private set; }
        public float Radius { get; private set; }

        public override CircleF Transform(Vector2 translaton, float rotation, Vector2 scale)
        {
            return new CircleF(Center + translaton, Radius * scale.X > scale.Y ? scale.X : scale.Y);
        }

        public override bool Contains(float x, float y)
        {
            var offset = new Vector2(x, y) - Center;
            return offset.LengthSquared() <= Radius * Radius;
        }
    }
}