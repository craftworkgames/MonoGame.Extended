using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class EntityManager : UpdateSystem
    {
        public EntityManager(ComponentManager componentManager)
        {
            _componentManager = componentManager;
            Entities = new Bag<Entity>(128);
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public Bag<Entity> Entities { get; }

        public event EventHandler<Entity> EntityAdded;
        public event EventHandler<int> EntityRemoved;

        public Entity CreateEntity()
        {
            // TODO: Recycle dead entites
            var id = _nextId++;
            var entity = new Entity(id, this, _componentManager);
            Entities[id] = entity;
            EntityAdded?.Invoke(this, entity);
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
        }
    }
}