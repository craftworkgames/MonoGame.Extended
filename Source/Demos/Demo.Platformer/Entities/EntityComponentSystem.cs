using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Platformer.Entities.Components;
using Demo.Platformer.Entities.Systems;
using Microsoft.Xna.Framework;

namespace Demo.Platformer.Entities
{
    public class EntityComponentSystem
    {
        public EntityComponentSystem() 
        {
            _entities = new List<Entity>();
            _components = new List<EntityComponent>();
            _systems = new List<ComponentSystem>();
            _nextEntityId = 1;
        }

        private readonly List<ComponentSystem> _systems;
        private readonly List<Entity> _entities;
        private readonly List<EntityComponent> _components;
        private long _nextEntityId;

        public void RegisterSystem(ComponentSystem system)
        {
            if (system.EntityComponentSystem != null)
                throw new InvalidOperationException("Component system already registered");

            system.EntityComponentSystem = this;
            _systems.Add(system);
        }

        public Entity CreateEntity(string name)
        {
            var entity = new Entity(this, _nextEntityId, name);
            _entities.Add(entity);
            _nextEntityId++;
            return entity;
        }

        public Entity FindEntity(string name)
        {
            return _entities.FirstOrDefault(e => e.Name == name);
        }

        public void DestroyEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void AttachComponent(EntityComponent component)
        {
            _components.Add(component);
        }

        public void DetachComponent(EntityComponent component)
        {
            _components.Remove(component);
        }

        internal IEnumerable<T> GetComponents<T>()
        {
            return _components.OfType<T>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var componentSystem in _systems.OfType<UpdatableComponentSystem>())
                componentSystem.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var componentSystem in _systems.OfType<DrawableComponentSystem>())
                componentSystem.Draw(gameTime);
        }
    }
}