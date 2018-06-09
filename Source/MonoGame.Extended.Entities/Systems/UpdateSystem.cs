using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class UpdateSystem : BaseSystem
    {
        protected UpdateSystem()
        {
        }

        public abstract void Update(GameTime gameTime);
    }
}