using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntityUpdateSystem : EntitySystem, IUpdateSystem
    {
        protected EntityUpdateSystem(AspectBuilder aspectBuilder) 
            : base(aspectBuilder)
        {
        }

        public abstract void Update(GameTime gameTime);
    }
}