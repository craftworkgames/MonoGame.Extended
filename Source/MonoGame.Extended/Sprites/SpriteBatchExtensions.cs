using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Sprites
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this Sprite sprite, SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, sprite);
        }
        
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite == null) throw new ArgumentNullException(nameof(sprite));

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                spriteBatch.Draw(texture, sprite.Position, sourceRectangle, sprite.Color * sprite.Alpha, sprite.Rotation, sprite.Origin,
                    sprite.Scale, sprite.Effect, 0);
            }
        }

        public static void Draw(this SpriteSheetAnimator animator, SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, animator);
        }

        public static void Draw(this SpriteBatch spriteBatch, SpriteSheetAnimator animator)
        {
            if (animator == null) throw new ArgumentNullException(nameof(animator));
            
            if (animator.IsPlaying && animator.Sprite != null)
                Draw(spriteBatch, animator.Sprite);
        }

        public static SpriteSheetAnimator CreateAnimator(this Sprite sprite, SpriteSheetAnimationGroup animationGroup)
        {
            return new SpriteSheetAnimator(animationGroup, sprite);
        }
    }
}