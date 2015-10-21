using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionInfo
    {
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public RectangleF IntersectingRectangle { get; internal set; }
        public Rectangle CellRectangle { get; internal set; }
    }
}