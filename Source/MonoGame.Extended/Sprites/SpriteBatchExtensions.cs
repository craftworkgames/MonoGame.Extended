using System;
using Microsoft.Xna.Framework;
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

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite, Vector2 offsetPosition, float offsetRotation, Vector2 offsetScale)
        {
            if (sprite == null) throw new ArgumentNullException(nameof(sprite));

            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;
                var position = offsetPosition + sprite.Position;
                var rotation = offsetRotation + sprite.Rotation;
                var scale = offsetScale * sprite.Scale;

                spriteBatch.Draw(texture, position, sourceRectangle, sprite.Color, rotation, sprite.Origin, 
                    scale, sprite.Effect, 0);
            }
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            Draw(spriteBatch, sprite, Vector2.Zero, 0, Vector2.One);
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