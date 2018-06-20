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

            _entityManager.EntityRemoved += OnEntityRemoved;
            _entityManager.EntityAdded += OnEntityAdded;
        }

        private void OnEntityAdded(object sender, int id) => _activeEntities.Add(id);
        private void OnEntityRemoved(object sender, int id) => _activeEntities.Remove(id);

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
                // TODO: Technically, many entities will probably share the same set of components. odb calls this the composition identity.
                if (_aspect.IsInterested(entity.ComponentBits))
                {
                    _activeEntities[count] = entity.Id;
                    count++;
                }
            }
        }
    }
}