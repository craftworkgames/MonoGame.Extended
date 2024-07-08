using Microsoft.Xna.Framework;

namespace MonoGame.Extended.ECS.Systems
{
    public abstract class EntityProcessingSystem : EntityUpdateSystem
    {
        protected EntityProcessingSystem(AspectBuilder aspectBuilder)
            : base(aspectBuilder)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Begin();

            foreach (var entityId in ActiveEntities)
                Process(gameTime, entityId);

            End();
        }

        public virtual void Begin() { }
        public abstract void Process(GameTime gameTime, int entityId);
        public virtual void End() { }
    }
}