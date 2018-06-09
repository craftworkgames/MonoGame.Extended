using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class DrawSystem : BaseSystem
    {
        protected DrawSystem()
        {
        }

        public abstract void Draw(GameTime gameTime);
    }
}