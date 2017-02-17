using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class TextureAtlasExtensions
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

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            var ninePatchRegion = textureRegion as NinePatchRegion2D;

            if (ninePatchRegion != null)
                Draw(spriteBatch, ninePatchRegion, destinationRectangle, color);
            else
            {
                var sourceRectangle = textureRegion.Bounds;

                if (clippingRectangle.HasValue)
                {
                    sourceRectangle = ClipSourceRectangle(sourceRectangle, destinationRectangle, clippingRectangle.Value);
                    destinationRectangle = ClipDestinationRectangle(destinationRectangle, clippingRectangle.Value);
                }

                spriteBatch.Draw(textureRegion.Texture, destinationRectangle, sourceRectangle, color);
            }
        }

        public static void Draw(this SpriteBatch spriteBatch, NinePatchRegion2D ninePatchRegion, Rectangle destinationRectangle, Color color)
        {
            var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
            var sourcePatches = ninePatchRegion.SourcePatches;

            for (var i = 0; i < sourcePatches.Length; i++)
                spriteBatch.Draw(ninePatchRegion.Texture, sourceRectangle: sourcePatches[i], destinationRectangle: destinationPatches[i], color: color);
        }

        private static Rectangle ClipSourceRectangle(Rectangle sourceRectangle, Rectangle destinationRectangle, Rectangle clippingRectangle)
        {
            var x = clippingRectangle.X > destinationRectangle.X ? clippingRectangle.X : destinationRectangle.X;
            var y = clippingRectangle.Y > destinationRectangle.Y ? clippingRectangle.Y : destinationRectangle.Y;
            var w = sourceRectangle.Width - 
                (clippingRectangle.Left > destinationRectangle.Left ? clippingRectangle.Left - destinationRectangle.Left : 0) - 
                (clippingRectangle.Right < destinationRectangle.Right ? destinationRectangle.Right - clippingRectangle.Right : 0);
            var h = sourceRectangle.Height - 
                (clippingRectangle.Top > destinationRectangle.Top ? clippingRectangle.Top - destinationRectangle.Top : 0) - 
                (clippingRectangle.Bottom < destinationRectangle.Bottom ? destinationRectangle.Bottom - clippingRectangle.Bottom : 0);

            return new Rectangle(sourceRectangle.X + x - destinationRectangle.X, sourceRectangle.Y + y -destinationRectangle.Y, w, h);
        }

        private static Rectangle ClipDestinationRectangle(Rectangle destinationRectangle, Rectangle clippingRectangle)
        {
            var left = clippingRectangle.Left < destinationRectangle.Left ? destinationRectangle.Left : clippingRectangle.Left;
            var top = clippingRectangle.Top < destinationRectangle.Top ? destinationRectangle.Top : clippingRectangle.Top;
            var bottom = clippingRectangle.Bottom < destinationRectangle.Bottom ? clippingRectangle.Bottom : destinationRectangle.Bottom;
            var right = clippingRectangle.Right < destinationRectangle.Right ? clippingRectangle.Right : destinationRectangle.Right;
            return new Rectangle(left, top, right - left, bottom - top);
        }

    }
}