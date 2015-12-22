using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public abstract class ShapeF<T>
    {
        protected ShapeF()
        {
        } 

        public abstract T Transform(Vector2 translaton, float rotation, Vector2 scale);
        public abstract bool Contains(float x, float y);

        public T Translate(Vector2 translation)
        {
            return Transform(translation, 0, Vector2.One);
        }

        public T Rotate(float rotation)
        {
            return Transform(Vector2.Zero, rotation, Vector2.One);
        }

        public T Scale(Vector2 scale)
        {
            return Transform(Vector2.Zero, 0, scale);
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}