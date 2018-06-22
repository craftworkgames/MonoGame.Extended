using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
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

            Entities = new Bag<Entity>(_defaultBagSize);
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public Bag<Entity> Entities { get; }

        private readonly Bag<int> _addedEntities;
        private readonly Bag<int> _removedEntities;
        private readonly Bag<int> _changedEntities;
        private readonly Bag<BitVector32> _entityToComponentBits;
        
        public event Action<int> EntityAdded;
        public event Action<int> EntityRemoved;
        public event Action<int> EntityChanged;

        public Entity CreateEntity()
        {
            // TODO: Recycle dead entites
            var id = _nextId++;
            var entity = new Entity(id, this, _componentManager);
            Entities[id] = entity;
            _addedEntities.Add(id);
            _entityToComponentBits[id] = new BitVector32(0);
            return entity;
        }

        public void DestroyEntity(int entityId)
        {
            EntityRemoved?.Invoke(entityId);
            throw new NotImplementedException();
            //Entities[entityId] = null;
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

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in _addedEntities)
            {
                _entityToComponentBits[entity] = _componentManager.GetComponentBits(entity);
                EntityAdded?.Invoke(entity);
            }

            foreach (var entity in _removedEntities)
            {
                _entityToComponentBits[entity] = default(BitVector32);
                EntityRemoved?.Invoke(entity);
            }

            foreach (var entity in _changedEntities)
            {
                _entityToComponentBits[entity] = _componentManager.GetComponentBits(entity);
                EntityChanged?.Invoke(entity);
            }

            _addedEntities.Clear();
            _removedEntities.Clear();
            _changedEntities.Clear();
        }
    }
}