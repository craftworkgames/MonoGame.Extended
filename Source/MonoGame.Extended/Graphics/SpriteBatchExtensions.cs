using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Graphics
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color)
        {
<<<<<<< HEAD
            var sourceRectangle = textureRegion.Bounds;
            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Color color) 
        {
            spriteBatch.Draw(sprite.Texture, sprite.Position, color);
=======
            spriteBatch.Draw(textureRegion.Texture, position, textureRegion.Bounds, color);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
>>>>>>> c01b03be82d27dfdf39b10c10f6f9a783180e398
        }
    }
}