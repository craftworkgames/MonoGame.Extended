using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntityDrawSystem : EntityUpdateSystem
    {
        protected EntityDrawSystem(Aspect.Builder aspect)
            : base(aspect)
        {
        }

        public sealed override void Update(GameTime gameTime)
        {
            Draw(gameTime);
        }

        public abstract void Draw(GameTime gameTime);
    }
}