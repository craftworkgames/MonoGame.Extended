using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.TextureAtlases
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color)
        {
            var sourceRectangle = textureRegion.Bounds;
            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite) 
        {
            if (sprite.IsVisible)
                spriteBatch.Draw(sprite.TextureRegion.Texture, null, sprite.GetBoundingRectangle(), sprite.TextureRegion.Bounds, sprite.Origin, sprite.Rotation, sprite.Scale, sprite.Color);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
        }
    }
}