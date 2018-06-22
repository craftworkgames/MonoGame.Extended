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

        internal EntitySubscription(EntityManager entityManager, Aspect aspect)
        {
            _entityManager = entityManager;
            _aspect = aspect;
            _activeEntities = new Bag<int>(entityManager.Entities.Capacity);
            _rebuildActives = true;

            _entityManager.EntityAdded += OnEntityAdded;
            _entityManager.EntityRemoved += OnEntityRemoved;
        }

        private void OnEntityAdded(int id) => _activeEntities.Add(id);
        private void OnEntityRemoved(int id) => _activeEntities.Remove(id);

        public void Dispose()
        {
            _entityManager.EntityAdded -= OnEntityAdded;
            _entityManager.EntityRemoved -= OnEntityRemoved;
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