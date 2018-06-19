using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntityUpdateSystem : UpdateSystem
    {
        protected EntityUpdateSystem(Aspect.Builder aspectBuilder)
        {
            _aspectBuilder = aspectBuilder;
        }

        private readonly Aspect.Builder _aspectBuilder;
        private EntitySubscription _subscription;

        private EntityWorld _world;
        public EntityWorld World
        {
            get => _world;
            internal set
            {
                _world = value;
                _subscription = new EntitySubscription(_world.EntityManager, _aspectBuilder.Build(_world.ComponentManager));

                // TODO: Undisposed events.
                _world.EntityManager.EntityAdded += (sender, entityId) => OnEntityAdded(entityId);
                _world.EntityManager.EntityRemoved += (sender, entityId) => OnEntityRemoved(entityId);
            }
        }

        protected virtual void OnEntityAdded(int entityId) { }
        protected virtual void OnEntityRemoved(int entityId) { }

        public Bag<int> ActiveEntities => _subscription.ActiveEntities;

        public abstract void Initialize(IComponentMapperService mapperService);
    }
}