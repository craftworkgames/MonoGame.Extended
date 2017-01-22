using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Entities
{
    public sealed class EntityComponentSystem : DrawableGameComponent
    {
        #region Private Variables

        private readonly Dictionary<Type, Func<object>> _componentFactories;
        private readonly Dictionary<string, ICollection<Type>> _entityDefinitions;

        private readonly HashSet<EntityComponent> _components;
        private readonly List<Guid> _entities;

        private readonly HashSet<EntitySystem> _systems;

        #endregion

        public EntityComponentSystem(Game game) 
            : base(game)
        {
            _components         = new HashSet<EntityComponent>();
            _componentFactories = new Dictionary<Type, Func<object>>();
            _entities           = new List<Guid>();
            _entityDefinitions  = new Dictionary<string, ICollection<Type>>();
            _systems            = new HashSet<EntitySystem>();
        }

        #region Entity Methods

        public void CreateEntity(string entityName, Action<Entity> initializer = null)
        {
            Guid entity = Guid.NewGuid();

            try
            {
                List<object> addedComponents = new List<object>();

                foreach (var type in _entityDefinitions[entityName])
                {
                    VerifyType(type);

                    var entityComponent = new EntityComponent()
                    {
                        Entity = entity,
                        Type = type,
                        Component = _componentFactories[type]()
                    };

                    addedComponents.Add(entityComponent.Component);
                    _components.Add(entityComponent);
                }

                Entity entityInst = entity.ToEntity(this);

                foreach (var system in _systems)
                {
                    system.EntityCreatedInternal(entityInst);
                    foreach (var component in addedComponents)
                        system.ComponentAddedInternal(entityInst, component);
                }

                initializer?.Invoke(entityInst);
                _entities.Add(entity);
            }
            catch (Exception)
            {
                _components.RemoveWhere(e => e.Entity == entity);
                throw;
            }
        }

        internal void DestroyEntity(Guid entity)
        {
            _entities.Remove(entity);
            ForEachSystem(s => s.EntityRemovedInternal(entity.ToEntity(this)));
            _components.RemoveWhere(e => e.Entity == entity);
        }

        internal bool EntityExists(Guid entity)
        {
            return _entities.Contains(entity);
        }

        internal void AddComponent(Guid entity, Type type, object component = null)
        {
            VerifyType(type);

            var entityComponent = new EntityComponent()
            {
                Entity = entity,
                Type = type,
                Component = component ?? _componentFactories[type]()
            };

            _components.Add(entityComponent);
            ForEachSystem(s => s.ComponentAddedInternal(entity.ToEntity(this), component));
        }

        internal void RemoveComponent(Guid entity, Type componentType, object component)
        {
            _components.RemoveWhere(e =>
            {
                if (e.Entity == entity && e.Type == componentType && e.Component == component)
                {
                    ForEachSystem(s => s.ComponentRemovedInternal(entity.ToEntity(this), e.Component));
                    return true;
                }
                return false;
            });
        }

        internal void RemoveComponents(Guid entity, Type type)
        {
            _components.RemoveWhere(e =>
            {
                if (e.Entity == entity && e.Type == type)
                {
                    ForEachSystem(s => s.ComponentRemovedInternal(entity.ToEntity(this), e.Component));
                    return true;
                }
                return false;
            });
        }

        internal object GetEntityComponent(Guid entity, Type componentType)
        {
            return _components.Where(c => c.Entity == entity && c.Type == componentType)
                .FirstOrDefault()
                .Component;
        }

        internal IEnumerable GetEntityComponents(Guid entity)
        {
            return from component in _components
                   where entity == component.Entity
                   select component.Component;
        }

        internal IEnumerable GetEntityComponents(Guid entity, Type componentType)
        {
            return from component in _components
                   where entity == component.Entity && componentType == component.Type
                   select component.Component;
        }

        #endregion

        #region Register Methods

        public void RegisterComponent<T>(Func<object> factory) => RegisterComponent(typeof(T), factory);
        public void RegisterComponent(Type componentType, Func<object> factory)
        {
            _componentFactories.Add(componentType, factory);
        }

        public void RegisterEntity(string entityName, ICollection<Type> components)
        {
            _entityDefinitions.Add(entityName, components);
        }

        public void RegisterSystem(EntitySystem system)
        {
            system.Ecs = this;
            _systems.Add(system);
        }

        #endregion

        #region DrawableGameComponent Methods

        public override void Initialize() => LoadContent();

        protected override void LoadContent() => ForEachSystem(s => s.LoadContentInternal(Game.Content));
        protected override void UnloadContent() => ForEachSystem(s => s.UnloadContentInternal());

        public override void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
            {
                foreach (var entity in _entities)
                    system.UpdateInternal(entity.ToEntity(this), gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
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

        private void VerifyType(Type type)
        {
            if (!_componentFactories.ContainsKey(type))
                throw new ArgumentException($"{type} is not a registered component");
        }

        private struct EntityComponent
        {
            public Guid Entity;
            public Type Type;
            public object Component;
        }

        #endregion
    }

    static class GuidExtensions
    {
        internal static Entity ToEntity(this Guid guid, EntityComponentSystem ecs) => new Entity(ecs, guid);
    }
}