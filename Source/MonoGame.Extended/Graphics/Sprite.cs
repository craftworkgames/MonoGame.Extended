using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Graphics
{
    public struct Sprite
    {
        public Point2F Centre;
        public Rectangle? SourceRectangle;
        public Color? Color;

        public Sprite(Point2F centre, Rectangle? sourceRectangle = null, Color? color = null)
        {
            Centre = centre;
            SourceRectangle = sourceRectangle;
            Color = color;
        }
    }
}
