using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteExtensions
    {
        public static void Draw(this Sprite sprite, SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 scale)
        {
            Draw(spriteBatch, sprite, position, rotation, scale);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Transform2 transform)
        {
            Draw(spriteBatch, sprite, transform.Position, transform.Rotation, transform.Scale);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, float rotation = 0)
        {
            Draw(spriteBatch, sprite, position, rotation, Vector2.One);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position, float rotation, Vector2 scale)
        {
            if (sprite == null) throw new ArgumentNullException(nameof(sprite));

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;
                spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color*sprite.Alpha, rotation, sprite.Origin, scale, sprite.Effect, sprite.Depth);
            }
        }
    }
}
