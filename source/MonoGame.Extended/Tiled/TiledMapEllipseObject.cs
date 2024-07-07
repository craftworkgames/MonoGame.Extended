using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapEllipseObject : TiledMapObject
    {
        public TiledMapEllipseObject(int identifier, string name, SizeF size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true, string type = null)
            : base(identifier, name, size, position, rotation, opacity, isVisible, type)
        {
            Radius = new Vector2(size.Width / 2.0f, size.Height / 2.0f);
            Center = new Vector2(position.X + Radius.X, position.Y);
        }

        public Vector2 Center { get; }
        public Vector2 Radius { get; }
    }
}
