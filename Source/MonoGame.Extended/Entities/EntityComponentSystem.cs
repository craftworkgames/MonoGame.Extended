using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Entities
{
    public sealed class EntityComponentSystem
    {
        #region Private Variables

        private readonly Dictionary<Type, ComponentDefinition> _componentDefinitions;
        private readonly Dictionary<string, EntityDefinition> _entityDefinitions;

        private readonly HashSet<EntityComponent> _components;
        private readonly List<Guid> _entities;

        private readonly HashSet<EntitySystem> _systems;

        #endregion

        public EntityComponentSystem()
        {
            _components           = new HashSet<EntityComponent>();
            _componentDefinitions = new Dictionary<Type, ComponentDefinition>();
            _entities             = new List<Guid>();
            _entityDefinitions    = new Dictionary<string, EntityDefinition>();
            _systems              = new HashSet<EntitySystem>();
        }

        #region Entity Methods

        public void CreateEntity(string entityName, Action<Entity> customFactory = null)
        {
            Guid entity = Guid.NewGuid();

            try
            {
                List<EntityComponent> addedComponents = new List<EntityComponent>();

                foreach (var type in _entityDefinitions[entityName].components)
                {
                    VerifyComponent(entity, type);

                    var entityComponent = new EntityComponent()
                    {
                        entity = entity,
                        type = type,
                        component = _componentDefinitions[type].factory()
                    };

                    addedComponents.Add(entityComponent);
                    _components.Add(entityComponent);
                }

                Entity entityInst = entity.ToEntity(this);

                foreach (var system in _systems)
                {
                    foreach (var component in addedComponents)
                        system.ComponentAddedInternal(entityInst, component.type, component.component);
                    system.EntityCreatedInternal(entityInst);
                }

                (customFactory ?? _entityDefinitions[entityName].factory)?.Invoke(entityInst);
                _entities.Add(entity);
            }
            catch (Exception)
            {
                _components.RemoveWhere(e => e.entity == entity);
                throw;
            }
        }

        internal void DestroyEntity(Guid entity)
        {
            if (_entities.Remove(entity))
            {
                Entity entityInst = entity.ToEntity(this);

                var components = from comp in _components
                                 where comp.entity == entity
                                 select comp;

                foreach (var comp in components)
                    ForEachSystem(s => s.ComponentRemovedInternal(entityInst, comp.type, comp.component));

                ForEachSystem(s => s.EntityRemovedInternal(entityInst));
                _components.RemoveWhere(c => components.Contains(c));
            }
        }

        internal bool EntityExists(Guid entity)
        {
            return _entities.Contains(entity);
        }

        internal void AddComponent(Guid entity, Type componentType, object component = null)
        {
            VerifyComponent(entity, componentType);

            var entityComponent = new EntityComponent()
            {
                entity = entity,
                type = componentType,
                component = component ?? _componentDefinitions[componentType].factory()
            };

            _components.Add(entityComponent);
            ForEachSystem(s => s.ComponentAddedInternal(entity.ToEntity(this), entityComponent.type, component));
        }

        internal void RemoveComponent(Guid entity, Type componentType, object component)
        {
            _components.RemoveWhere(e =>
            {
                if (e.entity == entity && e.type == componentType && e.component == component)
                {
                    ForEachSystem(s => s.ComponentRemovedInternal(entity.ToEntity(this), componentType, e.component));
                    return true;
                }
                return false;
            });
        }

        internal void RemoveComponents(Guid entity, Type componentType)
        {
            _components.RemoveWhere(e =>
            {
                if (e.entity == entity && e.type == componentType)
                {
                    ForEachSystem(s => s.ComponentRemovedInternal(entity.ToEntity(this), componentType, e.component));
                    return true;
                }
                return false;
            });
        }

        internal object GetEntityComponent(Guid entity, Type componentType)
        {
            return _components.Where(c => c.entity == entity && c.type == componentType)
                .FirstOrDefault()
                .component;
        }

        internal IEnumerable GetEntityComponents(Guid entity)
        {
            return from component in _components
                   where entity == component.entity
                   select component.component;
        }

        internal IEnumerable GetEntityComponents(Guid entity, Type componentType)
        {
            return from component in _components
                   where entity == component.entity && componentType == component.type
                   select component.component;
        }

        #endregion

        #region Register Methods

        public void RegisterComponent<T>(bool allowDuplicates = false) where T : new() 
            => RegisterComponent(typeof(T), () => new T(), allowDuplicates);

        public void RegisterComponent<T>(Func<object> factory, bool allowDuplicates = false) 
            => RegisterComponent(typeof(T), factory, allowDuplicates);

        public void RegisterComponent(Type componentType, Func<object> factory, bool allowDuplicates = false)
        {
            _componentDefinitions.Add(componentType, new ComponentDefinition()
            {
                factory = factory,
                allowDuplicates = allowDuplicates
            });
        }

        public void RegisterEntity(string entityName, ICollection<Type> components, Action<Entity> factory = null)
        {
            _entityDefinitions.Add(entityName, new EntityDefinition()
            {
                factory = factory,
                components = components
            });
        }

        public void RegisterSystem(EntitySystem system)
        {
            system.Ecs = this;
            _systems.Add(system);
        }

        #endregion

        #region DrawableGameComponent Methods

        public void Initialize() => ForEachSystem(s => s.InitializeInternal());

        public void LoadContent(ContentManager contentManager) => ForEachSystem(s => s.LoadContentInternal(contentManager));
        public void UnloadContent() => ForEachSystem(s => s.UnloadContentInternal());

        public void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
            {
                foreach (var entity in _entities)
                    system.UpdateInternal(entity.ToEntity(this), gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var system in _systems)
            {
                foreach (var entity in _entities)
                    system.DrawInternal(entity.ToEntity(this), gameTime);
            }
        }

        #endregion

        #region Utility

        private void ForEachSystem(Action<EntitySystem> action)
        {
            foreach (var system in _systems)
                action(system);
        }

        private void VerifyComponent(Guid entity, Type type)
        {
            if (!_componentDefinitions.ContainsKey(type))
                throw new ArgumentException($"{type.Name} is not a registered component");

            if (_componentDefinitions[type].allowDuplicates)
            {
                int count = _components.Where(c => c.entity == entity && c.type == type).Count();
                if (count > 1)
                    throw new ArgumentException($"{type.Name} already exists on the entity");
            }
        }

        private struct ComponentDefinition
        {
            public Func<object> factory;
            public bool allowDuplicates;
        }

        private struct EntityComponent
        {
            public Guid entity;
            public Type type;
            public object component;
        }

        private struct EntityDefinition
        {
            public IEnumerable<Type> components;
            public Action<Entity> factory;
        }

        #endregion
    }

    static class GuidExtensions
    {
        internal static Entity ToEntity(this Guid guid, EntityComponentSystem ecs) => new Entity(ecs, guid);
    }
}