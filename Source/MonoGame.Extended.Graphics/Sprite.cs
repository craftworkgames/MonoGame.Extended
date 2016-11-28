using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public struct Sprite
    {
        public Texture2D Texture;
        public Rectangle? Source;
        public Color? Color;
        public SizeF? Size;
        public SpriteEffects SpriteEffects;
    }
}
