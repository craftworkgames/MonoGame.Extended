using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class EntityUpdateSystem : UpdateSystem
    {
        protected EntityUpdateSystem(AspectBuilder aspectBuilder)
        {
            _aspectBuilder = aspectBuilder;
        }

        public override void Dispose()
        {
            if (_world != null)
            {
                _world.EntityManager.EntityAdded -= OnEntityAdded;
                _world.EntityManager.EntityRemoved -= OnEntityRemoved;
            }

            base.Dispose();
        }

        private readonly AspectBuilder _aspectBuilder;
        private EntitySubscription _subscription;

        private EntityWorld _world;
        public EntityWorld World
        {
            get => _world;
            internal set
            {
                _world = value;
                _subscription = new EntitySubscription(_world.EntityManager, _aspectBuilder.Build(_world.ComponentManager));
                _world.EntityManager.EntityAdded += OnEntityAdded;
                _world.EntityManager.EntityRemoved += OnEntityRemoved;
            }
        }

        protected virtual void OnEntityAdded(int entityId) { }
        protected virtual void OnEntityRemoved(int entityId) { }

        public Bag<int> ActiveEntities => _subscription.ActiveEntities;

        public abstract void Initialize(IComponentMapperService mapperService);
    }
}