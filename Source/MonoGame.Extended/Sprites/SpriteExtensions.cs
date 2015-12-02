using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite == null) throw new ArgumentNullException("sprite");

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                spriteBatch.Draw(texture, sprite.Position, sourceRectangle, sprite.Color, sprite.Rotation, sprite.Origin,
                    sprite.Scale, sprite.Effect, 0);
            }
        }

        public static void Draw(this SpriteBatch spriteBatch, SpriteSheetAnimator animator)
        {
            if (animator == null) throw new ArgumentNullException("animator");

            if (animator.Sprite != null)
                Draw(spriteBatch, animator.Sprite);
        }

        public static SpriteSheetAnimator CreateAnimator(this Sprite sprite, IEnumerable<TextureRegion2D> regions)
        {
            return new SpriteSheetAnimator(sprite, regions);
        }
    }
}