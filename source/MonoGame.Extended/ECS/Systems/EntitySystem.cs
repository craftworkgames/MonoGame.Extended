using MonoGame.Extended.Collections;

namespace MonoGame.Extended.ECS.Systems
{
    public abstract class EntitySystem : ISystem
    {
        protected EntitySystem(AspectBuilder aspectBuilder)
        {
            _aspectBuilder = aspectBuilder;
        }

        public void Dispose()
        {
            if (_world != null)
            {
                _world.EntityManager.EntityAdded -= OnEntityAdded;
                _world.EntityManager.EntityRemoved -= OnEntityRemoved;
            }
        }

        private readonly AspectBuilder _aspectBuilder;
        private EntitySubscription _subscription;

        private World _world;

        protected virtual void OnEntityChanged(int entityId) { }
        protected virtual void OnEntityAdded(int entityId) { }
        protected virtual void OnEntityRemoved(int entityId) { }

        public Bag<int> ActiveEntities => _subscription.ActiveEntities;

        public virtual void Initialize(World world)
        {
            _world = world;

            var aspect = _aspectBuilder.Build(_world.ComponentManager);
            _subscription = new EntitySubscription(_world.EntityManager, aspect);
            _world.EntityManager.EntityAdded += OnEntityAdded;
            _world.EntityManager.EntityRemoved += OnEntityRemoved;
            _world.EntityManager.EntityChanged += OnEntityChanged;

            Initialize(world.ComponentManager);
        }

        public abstract void Initialize(IComponentMapperService mapperService);

        protected void DestroyEntity(int entityId) => _world.DestroyEntity(entityId);
        protected Entity CreateEntity() => _world.CreateEntity();
        protected Entity GetEntity(int entityId) => _world.GetEntity(entityId);
    }
}