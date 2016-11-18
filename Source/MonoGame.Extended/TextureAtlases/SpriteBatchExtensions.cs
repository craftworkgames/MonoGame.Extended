using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position,
            Color color)
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
            var ninePatchRegion = textureRegion as NinePatchRegion2D;

            if (ninePatchRegion != null)
                Draw(spriteBatch, ninePatchRegion, destinationRectangle, color);
            else
                spriteBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, NinePatchRegion2D ninePatchRegion, Rectangle destinationRectangle, Color color)
        {
            var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
            var sourcePatches = ninePatchRegion.SourcePatches;

            for (var i = 0; i < sourcePatches.Length; i++)
                spriteBatch.Draw(ninePatchRegion.Texture, sourceRectangle: sourcePatches[i], destinationRectangle: destinationPatches[i], color: color);
        }
    }
}