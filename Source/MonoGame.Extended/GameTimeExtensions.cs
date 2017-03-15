using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class GameTimeExtensions
    {
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}