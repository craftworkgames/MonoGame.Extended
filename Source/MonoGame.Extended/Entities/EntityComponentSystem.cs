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
            _entitiesByName = new Dictionary<string, Entity>();
            _dyingEntities = new List<DyingEntity>();
            _components = new List<EntityComponent>();
            _systems = new List<ComponentSystem>();
            _nextEntityId = 1;
        }

        private readonly List<ComponentSystem> _systems;
        private readonly List<Entity> _entities;
        private readonly List<EntityComponent> _components;
        private readonly Dictionary<string, Entity> _entitiesByName;
        private readonly List<DyingEntity> _dyingEntities;
        private long _nextEntityId;

        internal event EventHandler<EntityComponent> ComponentAttached;
        internal event EventHandler<EntityComponent> ComponentDetached;
        internal event EventHandler<Entity> EntityCreated;
        internal event EventHandler<Entity> EntityDestroyed;

        public class DyingEntity
        {
            public float SecondsUntilDeath;
            public Entity Entity;
        }

        public void RegisterSystem(ComponentSystem system)
        {
            if (system.Parent != null)
                throw new InvalidOperationException("Component system already registered");

            system.Parent = this;
            _systems.Add(system);
        }

        public Entity CreateEntity()
        {
            return CreateEntity(null);
        }

        public Entity CreateEntity(Vector2 position, float rotation = 0)
        {
            return CreateEntity(null, position, rotation);
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

            if (name != null)
                _entitiesByName.Add(name, entity);

            EntityCreated?.Invoke(this, entity);
            _nextEntityId++;
            return entity;
        }

        public void DestroyEntity(Entity entity, float delaySeconds)
        {
            if (delaySeconds.Equals(0f))
                DestroyEntity(entity);
            else
                _dyingEntities.Add(new DyingEntity {Entity = entity, SecondsUntilDeath = delaySeconds});
        }

        public void DestroyEntity(Entity entity)
        {
            foreach (var component in _components.Where(c => c.Entity == entity).OfType<IDisposable>())
                component.Dispose();

            _components.RemoveAll(c => c.Entity == entity);

            if (entity.Name != null)
                _entitiesByName.Remove(entity.Name);

            _entities.Remove(entity);
            EntityDestroyed?.Invoke(this, entity);
        }

        public Entity GetEntity(string name)
        {
            Entity entity;
            return _entitiesByName.TryGetValue(name, out entity) ? entity : null;
        }

        internal void AttachComponent(EntityComponent component)
        {
            _components.Add(component);
            ComponentAttached?.Invoke(this, component);
        }

        internal void DetachComponent(EntityComponent component)
        {
            _components.Remove(component);
            ComponentDetached?.Invoke(this, component);
        }

        internal IEnumerable<T> GetComponents<T>()
        {
            return _components.OfType<T>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var componentSystem in _systems)
                componentSystem.Update(gameTime);

            for (var i = 0; i < _dyingEntities.Count; i++)
            {
                _dyingEntities[i].SecondsUntilDeath -= gameTime.GetElapsedSeconds();

                if (_dyingEntities[i].SecondsUntilDeath <= 0)
                {
                    DestroyEntity(_dyingEntities[i].Entity);
                    _dyingEntities.Remove(_dyingEntities[i]);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var componentSystem in _systems)
                componentSystem.Draw(gameTime);
        }
    }
}