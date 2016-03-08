using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public interface IShapeF
    {
        float Bottom { get; }
        float Left { get; }
        float Right { get; }
        float Top { get; }

        RectangleF GetBoundingRectangle();

        bool Contains(float x, float y);
        bool Contains(Vector2 point);
    }
}