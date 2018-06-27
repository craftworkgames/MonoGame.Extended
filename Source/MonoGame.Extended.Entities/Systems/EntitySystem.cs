using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntitySystem : ISystem
    {
        protected EntitySystem(AspectBuilder aspectBuilder)
        {
            _aspectBuilder = aspectBuilder;
        }

        public void Dispose()
        {
            if (World != null)
            {
                World.EntityManager.EntityAdded -= OnEntityAdded;
                World.EntityManager.EntityRemoved -= OnEntityRemoved;
            }
        }

        public virtual void Initialize(EntityWorld world)
        {
            World = world;

            var aspect = _aspectBuilder.Build(World.ComponentManager);
            _subscription = new EntitySubscription(World.EntityManager, aspect);
            World.EntityManager.EntityAdded += OnEntityAdded;
            World.EntityManager.EntityRemoved += OnEntityRemoved;
            World.EntityManager.EntityChanged += OnEntityChanged;

            Initialize(world.ComponentManager);
        }

        public abstract void Initialize(IComponentMapperService mapperService);

        private readonly AspectBuilder _aspectBuilder;
        private EntitySubscription _subscription;

        public EntityWorld World { get; private set; }

        protected virtual void OnEntityChanged(int entityId) { }
        protected virtual void OnEntityAdded(int entityId) { }
        protected virtual void OnEntityRemoved(int entityId) { }

        public Bag<int> ActiveEntities => _subscription.ActiveEntities;
    }
}