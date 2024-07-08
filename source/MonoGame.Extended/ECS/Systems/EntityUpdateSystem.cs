using Microsoft.Xna.Framework;

namespace MonoGame.Extended.ECS.Systems
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