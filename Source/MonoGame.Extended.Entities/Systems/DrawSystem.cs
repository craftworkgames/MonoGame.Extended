using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class DrawSystem : BaseSystem
    {
        protected DrawSystem(Aspect.Builder aspect)
            : base(aspect)
        {
        }

        public abstract void Draw(GameTime gameTime);
    }
}