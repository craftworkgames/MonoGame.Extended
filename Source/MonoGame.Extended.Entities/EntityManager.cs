using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class EntityManager : IUpdateSystem
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

            Entities = new Bag<Entity>(_defaultBagSize);

            _entityPool = new Pool<Entity>(() => new Entity(_nextId++, this, _componentManager), _defaultBagSize);
        }

        public void Dispose()
        {
        }

        public void Initialize(World world)
        {
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public Bag<Entity> Entities { get; }

        private readonly Pool<Entity> _entityPool;
        private readonly Bag<int> _addedEntities;
        private readonly Bag<int> _removedEntities;
        private readonly Bag<int> _changedEntities;
        private readonly Bag<BitVector32> _entityToComponentBits;
        
        public event Action<int> EntityAdded;
        public event Action<int> EntityRemoved;
        public event Action<int> EntityChanged;

        public Entity CreateEntity()
        {
            var entity = _entityPool.Obtain();
            var id = entity.Id;
            Entities[id] = entity;
            _addedEntities.Add(id);
            _entityToComponentBits[id] = new BitVector32(0);
            return entity;
        }

        public void DestroyEntity(int entityId)
        {
            _removedEntities.Add(entityId);
            _entityToComponentBits[entityId] = default(BitVector32);
            _entityPool.Free(Entities[entityId]);
            Entities[entityId] = null;
        }

        public void DestroyEntity(Entity entity)
        {
            DestroyEntity(entity.Id);
        }
        
        public BitVector32 GetComponentBits(int entityId)
        {
            return _entityToComponentBits[entityId];
        }

        private void OnComponentsChanged(int entityId)
        {
            _changedEntities.Add(entityId);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _addedEntities)
            {
                _entityToComponentBits[entity] = _componentManager.CreateComponentBits(entity);
                EntityAdded?.Invoke(entity);
            }

            foreach (var entity in _changedEntities)
            {
                _entityToComponentBits[entity] = _componentManager.CreateComponentBits(entity);
                EntityChanged?.Invoke(entity);
            }

            foreach (var entity in _removedEntities)
            {
                _entityToComponentBits[entity] = default(BitVector32);
                EntityRemoved?.Invoke(entity);
            }

            _addedEntities.Clear();
            _removedEntities.Clear();
            _changedEntities.Clear();
        }
    }
}