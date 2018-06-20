using System;
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
            _newEntities = new Bag<int>(_defaultBagSize);
            _removedEntities = new Bag<int>(_defaultBagSize);

            Entities = new Bag<Entity>(_defaultBagSize);
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public Bag<Entity> Entities { get; }

        private readonly Bag<int> _newEntities;
        private readonly Bag<int> _removedEntities;

        public event EventHandler<int> EntityAdded;
        public event EventHandler<int> EntityRemoved;

        public Entity CreateEntity()
        {
            // TODO: Recycle dead entites
            var id = _nextId++;
            var entity = new Entity(id, this, _componentManager);
            Entities[id] = entity;
            _newEntities.Add(id);
            return entity;
        }

        public void DestroyEntity(int entityId)
        {
            EntityRemoved?.Invoke(this, entityId);
            throw new NotImplementedException();
            //Entities[entityId] = null;
        }

        public void DestroyEntity(Entity entity)
        {
            DestroyEntity(entity.Id);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var newEntity in _newEntities)
                EntityAdded?.Invoke(this, newEntity);

            foreach (var removedEntity in _removedEntities)
                EntityRemoved?.Invoke(this, removedEntity);

            _newEntities.Clear();
            _removedEntities.Clear();
        }
    }
}