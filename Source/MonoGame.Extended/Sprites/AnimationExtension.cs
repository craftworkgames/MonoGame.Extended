using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Sprites
{
    public static class AnimationExtension
    {
        public static void Draw(this SpriteBatch spriteBatch, Animation animation)
        {
            spriteBatch.Draw(animation._sprites[animation.FrameIndex]);
        }
    }
}

