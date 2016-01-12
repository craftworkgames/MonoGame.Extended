using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;

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
            
            if (animator.IsPlaying && animator.Sprite != null)
                Draw(spriteBatch, animator.Sprite);
        }

        public static SpriteSheetAnimator CreateAnimator(this Sprite sprite, SpriteSheetAnimationGroup animationGroup)
        {
            return new SpriteSheetAnimator(animationGroup, sprite);
        }
    }
}