using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                spriteBatch.Draw(texture, sprite.Position, sourceRectangle, sprite.Color, sprite.Rotation, sprite.Origin,
                    sprite.Scale, sprite.Effect, 0);
            }
        }
    }
}