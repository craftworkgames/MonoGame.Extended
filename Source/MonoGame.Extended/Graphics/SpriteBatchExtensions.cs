using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color)
        {
            var sourceRectangle = textureRegion.Bounds;
            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color);
        }
    }
}