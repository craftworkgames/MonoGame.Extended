using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public interface IRectangular
    {
        Rectangle BoundingRectangle { get; }
    }

    public interface IRectangularF
    {
        RectangleF BoundingRectangle { get; }
    }
}
