using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
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

        public Entity CreateEntity(string name, Vector2 position, float rotation = 0)
        {
            var entity = CreateEntity(name);
            entity.Position = position;
            entity.Rotation = rotation;
            return entity;
        }

        public Entity CreateEntity(string name)
        {
            var entity = new Entity(this, _nextEntityId, name);
            _entities.Add(entity);
            _nextEntityId++;
            return entity;
        }

        public Entity GetEntity(string name)
        {
            return _entities.FirstOrDefault(e => e.Name == name);
        }

        public void DestroyEntity(Entity entity)
        {
            _entities.Remove(entity);
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

        internal T GetComponent<T>(Entity entity) where T : EntityComponent
        {
            return _components.OfType<T>().FirstOrDefault(i => i.Entity == entity);
        }

        internal void AttachComponent(Entity entity, EntityComponent component)
        {
            if (component.Entity != null)
                throw new InvalidOperationException("Component already attached to another entity");

            component.Entity = entity;
            _components.Add(component);
        }

        internal void DetachComponent(Entity entity, EntityComponent component)
        {
            if (component.Entity != entity)
                throw new InvalidOperationException("Component not attached to entity");

            component.Entity = null;
            _components.Remove(component);
        }

        internal IEnumerable<T> GetComponents<T>()
        {
            return _components.OfType<T>();
        }
    }
}