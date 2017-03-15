using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public static class TextureAtlasExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
        {
            Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Rectangle? clippingRectangle = null)
        {
            var sourceRectangle = textureRegion.Bounds;

            if (clippingRectangle.HasValue)
            {
                var x = (int)(position.X - origin.X);
                var y = (int)(position.Y - origin.Y);
                var width = (int)(textureRegion.Width * scale.X);
                var height = (int)(textureRegion.Height * scale.Y);
                var destinationRectangle = new Rectangle(x, y, width, height);

                sourceRectangle = ClipSourceRectangle(textureRegion.Bounds, destinationRectangle, clippingRectangle.Value);
                position.X += sourceRectangle.X - textureRegion.Bounds.X;
                position.Y += sourceRectangle.Y - textureRegion.Bounds.Y;

                if(sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                    return;
            }

            spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            var ninePatchRegion = textureRegion as NinePatchRegion2D;

            if (ninePatchRegion != null)
                Draw(spriteBatch, ninePatchRegion, destinationRectangle, color, clippingRectangle);
            else
                Draw(spriteBatch, textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, NinePatchRegion2D ninePatchRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            var destinationPatches = ninePatchRegion.CreatePatches(destinationRectangle);
            var sourcePatches = ninePatchRegion.SourcePatches;

            for (var i = 0; i < sourcePatches.Length; i++)
            {
                var sourcePatch = sourcePatches[i];
                var destinationPatch = destinationPatches[i];

                if (clippingRectangle.HasValue)
                {
                    sourcePatch = ClipSourceRectangle(sourcePatch, destinationPatch, clippingRectangle.Value);
                    destinationPatch = ClipDestinationRectangle(destinationPatch, clippingRectangle.Value);
                    Draw(spriteBatch, ninePatchRegion.Texture, sourcePatch, destinationPatch, color, clippingRectangle);
                }
                else
                {
                    if (destinationPatch.Width > 0 && destinationPatch.Height > 0)
                        spriteBatch.Draw(ninePatchRegion.Texture, sourceRectangle: sourcePatch, destinationRectangle: destinationPatch, color: color);
                }
            }
        }

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle)
        {
            if (clippingRectangle.HasValue)
            {
                sourceRectangle = ClipSourceRectangle(sourceRectangle, destinationRectangle, clippingRectangle.Value);
                destinationRectangle = ClipDestinationRectangle(destinationRectangle, clippingRectangle.Value);
            }

            if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        private static Rectangle ClipSourceRectangle(Rectangle sourceRectangle, Rectangle destinationRectangle, Rectangle clippingRectangle)
        {
            var left = (float)(clippingRectangle.Left - destinationRectangle.Left);
            var right = (float)(destinationRectangle.Right - clippingRectangle.Right);
            var top = (float)(clippingRectangle.Top - destinationRectangle.Top);
            var bottom = (float)(destinationRectangle.Bottom - clippingRectangle.Bottom);
            var x =  left > 0 ? left : 0;
            var y = top > 0 ? top : 0;
            var w = (right > 0 ? right : 0) + x;
            var h = (bottom > 0 ? bottom : 0) + y;

            var scaleX = (float)destinationRectangle.Width / sourceRectangle.Width;
            var scaleY = (float)destinationRectangle.Height / sourceRectangle.Height;
            x /= scaleX;
            y /= scaleY;
            w /= scaleX;
            h /= scaleY;

            return new Rectangle((int)(sourceRectangle.X + x), (int)(sourceRectangle.Y + y), (int)(sourceRectangle.Width - w), (int)(sourceRectangle.Height - h));
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