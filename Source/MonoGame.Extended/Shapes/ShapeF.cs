using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public abstract class ShapeF
    {
        public abstract bool Contains(float x, float y);

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}