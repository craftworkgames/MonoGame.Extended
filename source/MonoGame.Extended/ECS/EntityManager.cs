using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS
{
    public class EntityManager : UpdateSystem
    {
        private const int _defaultBagSize = 128;

        public EntityManager(ComponentManager componentManager)
        {
            _componentManager = componentManager;
            _addedEntities = new Bag<int>(_defaultBagSize);
            _removedEntities = new Bag<int>(_defaultBagSize);
            _changedEntities = new Bag<int>(_defaultBagSize);
            _entityToComponentBits = new Bag<BitVector32>(_defaultBagSize);
            _componentManager.ComponentsChanged += OnComponentsChanged;

            _entityBag = new Bag<Entity>(_defaultBagSize);
            _entityPool = new Pool<Entity>(() => new Entity(_nextId++, this, _componentManager), _defaultBagSize);
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public int Capacity => _entityBag.Capacity;
        public IEnumerable<int> Entities => _entityBag.Where(e => e != null).Select(e => e.Id);
        public int ActiveCount { get; private set; }

        private readonly Bag<Entity> _entityBag;
        private readonly Pool<Entity> _entityPool;
        private readonly Bag<int> _addedEntities;
        private readonly Bag<int> _removedEntities;
        private readonly Bag<int> _changedEntities;
        private readonly Bag<BitVector32> _entityToComponentBits;

        public event Action<int> EntityAdded;
        public event Action<int> EntityRemoved;
        public event Action<int> EntityChanged;

        public Entity Create()
        {
            var entity = _entityPool.Obtain();
            var id = entity.Id;
            Debug.Assert(_entityBag[id] == null);
            _entityBag[id] = entity;
            _addedEntities.Add(id);
            _entityToComponentBits[id] = new BitVector32(0);
            return entity;
        }

        public void Destroy(int entityId)
        {
            if (!_removedEntities.Contains(entityId))
                _removedEntities.Add(entityId);
        }

        public void Destroy(Entity entity)
        {
            Destroy(entity.Id);
        }

        public Entity Get(int entityId)
        {
            return _entityBag[entityId];
        }

        public BitVector32 GetComponentBits(int entityId)
        {
            return _entityToComponentBits[entityId];
        }

        private void OnComponentsChanged(int entityId)
        {
            _changedEntities.Add(entityId);
            _entityToComponentBits[entityId] = _componentManager.CreateComponentBits(entityId);
            EntityChanged?.Invoke(entityId);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in _addedEntities)
            {
                _entityToComponentBits[entityId] = _componentManager.CreateComponentBits(entityId);
                ActiveCount++;
                EntityAdded?.Invoke(entityId);
            }

            foreach (var entityId in _changedEntities)
            {
                _entityToComponentBits[entityId] = _componentManager.CreateComponentBits(entityId);
                EntityChanged?.Invoke(entityId);
            }

            foreach (var entityId in _removedEntities)
            {
                // we must notify subscribers before removing it from the pool
                // otherwise an entity system could still be using the entity when the same id is obtained.
                EntityRemoved?.Invoke(entityId);

                var entity = _entityBag[entityId];
                _entityBag[entityId] = null;
                _componentManager.Destroy(entityId);
                _entityToComponentBits[entityId] = default(BitVector32);
                ActiveCount--;
                _entityPool.Free(entity);
            }

            _addedEntities.Clear();
            _removedEntities.Clear();
            _changedEntities.Clear();
        }
    }
}
