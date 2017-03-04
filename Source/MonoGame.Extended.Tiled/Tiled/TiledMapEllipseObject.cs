using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapEllipseObject : TiledMapObject
    {
        public Vector2 Center { get; }
        public float RadiusX { get; }
        public float RadiusY { get; }

        internal TiledMapEllipseObject(ContentReader input) 
            : base(input)
        {
            RadiusX = Size.Width / 2.0f;
            RadiusY = Size.Height / 2.0f;
            Center = new Vector2(Position.X + RadiusX, Position.Y);
        }
    }
}
