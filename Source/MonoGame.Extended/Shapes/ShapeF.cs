using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public interface IShapeF
    {
        float Left { get; }
        float Top { get; }
        float Right { get; }
        float Bottom { get; }
        RectangleF BoundingRectangle { get; }
        bool Contains(float x, float y);
        bool Contains(Vector2 point);
    }
}