using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(sprite, Vector2.Zero);
        }

        internal static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 position)
        {
            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                spriteBatch.Draw(texture, sprite.Position + position, sourceRectangle, sprite.Color, sprite.Rotation, sprite.Origin,
                    sprite.Scale, sprite.Effect, 0);
            }
        }
    }
}