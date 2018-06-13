using System;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public class EntitySubscription : IDisposable
    {
        private readonly Bag<int> _activeEntities;
        private readonly EntityManager _entityManager;
        private readonly Aspect _aspect;
        private readonly bool _rebuildActives;

        public EntitySubscription(EntityManager entityManager, Aspect aspect)
        {
            _entityManager = entityManager;
            _aspect = aspect;
            _activeEntities = new Bag<int>(entityManager.Entities.Capacity);
            _rebuildActives = true;

            _entityManager.EntityRemoved += OnEntityRemoved;
            _entityManager.EntityAdded += OnEntityAdded;
        }

        // TODO: It's a bit heavy handed to rebuild the actives every time one entity is added or removed.
        private void OnEntityAdded(object sender, Entity e) => RebuildActives();
        private void OnEntityRemoved(object sender, int e) => RebuildActives();

        public void Dispose()
        {
        }

        public Bag<int> ActiveEntities
        {
            get
            {
                if (_rebuildActives)
                    RebuildActives();

                return _activeEntities;
            }
        }

        private void RebuildActives()
        {
            var count = 0;
            _activeEntities.Clear();

            foreach (var entity in _entityManager.Entities)
            {
                if (_aspect.IsInterested(entity.ComponentBits))
                {
                    _activeEntities[count] = entity.Id;
                    count++;
                }
            }
        }
    }
}