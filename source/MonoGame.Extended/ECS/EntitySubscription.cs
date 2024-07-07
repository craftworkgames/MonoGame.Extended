using System;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.ECS
{
    internal class EntitySubscription : IDisposable
    {
        private readonly Bag<int> _activeEntities;
        private readonly EntityManager _entityManager;
        private readonly Aspect _aspect;
        private bool _rebuildActives;

        internal EntitySubscription(EntityManager entityManager, Aspect aspect)
        {
            _entityManager = entityManager;
            _aspect = aspect;
            _activeEntities = new Bag<int>(entityManager.Capacity);
            _rebuildActives = true;

            _entityManager.EntityAdded += OnEntityAdded;
            _entityManager.EntityRemoved += OnEntityRemoved;
            _entityManager.EntityChanged += OnEntityChanged;
        }

        private void OnEntityAdded(int entityId)
        {
            if (_aspect.IsInterested(_entityManager.GetComponentBits(entityId)))
                _activeEntities.Add(entityId);
        }

        private void OnEntityRemoved(int entityId) => _rebuildActives = true;
        private void OnEntityChanged(int entityId) => _rebuildActives = true;

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
            _activeEntities.Clear();

            foreach (var entity in _entityManager.Entities)
                OnEntityAdded(entity);

            _rebuildActives = false;
        }
    }
}