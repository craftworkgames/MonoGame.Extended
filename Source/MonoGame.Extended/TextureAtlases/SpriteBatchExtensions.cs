using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color)
        {
            spriteBatch.Draw(textureRegion.Texture, position, textureRegion.Bounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
        }
    }
}