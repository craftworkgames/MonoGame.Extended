using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite.IsVisible)
                spriteBatch.Draw(sprite.TextureRegion.Texture, null, sprite.GetBoundingRectangle(), sprite.TextureRegion.Bounds, sprite.Origin, sprite.Rotation, sprite.Scale, sprite.Color);
        }
    }
}