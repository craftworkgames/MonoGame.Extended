using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace Pong.GameObjects
{
    public class GameObject
    {
        public Vector2 Position;
        public Sprite Sprite;
        public RectangleF BoundingRectangle => Sprite.GetBoundingRectangle(Position, 0, Vector2.One);
    }
}