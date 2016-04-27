using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public abstract class Animation : IUpdate
    {
        public abstract void Update(GameTime gameTime);
    }
}