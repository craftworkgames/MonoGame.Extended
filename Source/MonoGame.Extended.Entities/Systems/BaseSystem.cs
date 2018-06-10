using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class BaseSystem
    {
        private readonly Aspect.Builder _aspectBuilder;

        protected BaseSystem(Aspect.Builder aspect)
        {
            _aspectBuilder = aspect;
        }

        protected Aspect Aspect { get; private set; }

        public EntityWorld  World { get; internal set; }

        public abstract void Initialize(ComponentManager componentManager);

        protected Bag<Entity> GetEntities()
        {
            var aspect = _aspectBuilder.Build(World.ComponentManager);
            return World.EntityManager.Entities;
        }
    }
}