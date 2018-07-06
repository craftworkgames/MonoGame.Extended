using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntityDrawSystem : EntitySystem, IDrawSystem
    {
        protected EntityDrawSystem(AspectBuilder aspect)
            : base(aspect)
        {
        }

        public abstract void Draw(GameTime gameTime);
    }
}