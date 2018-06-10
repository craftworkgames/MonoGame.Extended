using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class UpdateSystem : BaseSystem
    {
        protected UpdateSystem(Aspect.Builder aspect)
            : base(aspect)
        {
        }

        public abstract void Update(GameTime gameTime);
    }
}