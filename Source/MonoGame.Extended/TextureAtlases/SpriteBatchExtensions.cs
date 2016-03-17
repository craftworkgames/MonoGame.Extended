using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color)
        {
            var sourceRectangle = textureRegion.Bounds;
            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color, 
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            var sourceRectangle = textureRegion.Bounds;
            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
        }
    }
}