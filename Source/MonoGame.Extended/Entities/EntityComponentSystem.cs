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

        private readonly HashSet<IEntitySystem> _systems;

        #endregion

        public EntityComponentSystem(Game game) 
            : base(game)
        {
            _components         = new HashSet<EntityComponent>();
            _componentFactories = new Dictionary<Type, Func<object>>();
            _entities           = new List<Guid>();
            _entityDefinitions  = new Dictionary<string, ICollection<Type>>();
            _systems            = new HashSet<IEntitySystem>();
        }

        #region Entity Methods

        public void CreateEntity(string entityName)
        {
            Guid entity = Guid.NewGuid();

            try
            {
                List<object> addedComponents = new List<object>();

                foreach (var type in _entityDefinitions[entityName])
                {
                    var entityComponent = new EntityComponent()
                    {
                        Entity = entity,
                        Type = type,
                        Component = _componentFactories[type]()
                    };

                    addedComponents.Add(entityComponent.Component);
                    _components.Add(entityComponent);
                }

                foreach (var system in _systems)
                {
                    system.EntityCreated(entity.ToEntity(this));
                    foreach (var component in addedComponents)
                        system.ComponentAdded(entity.ToEntity(this), component);
                }

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
            ForEachSystem(s => s.EntityRemoved(entity.ToEntity(this)));
            _components.RemoveWhere(e => e.Entity == entity);
        }

        internal void AddComponent(Guid entity, Type type)
        {
            var entityComponent = new EntityComponent()
            {
                Entity = entity,
                Type = type,
                Component = _componentFactories[type]()
            };

            _components.Add(entityComponent);
            ForEachSystem(s => s.ComponentAdded(new Entity(this, entity), entityComponent.Component));
        }

        internal void RemoveComponent(Guid entity, Type componentType, object component)
        {
            _components.RemoveWhere(e =>
            {
                if (e.Entity == entity && e.Type == componentType && e.Component == component)
                {
                    ForEachSystem(s => s.ComponentRemoved(entity.ToEntity(this), e.Component));
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
                    ForEachSystem(s => s.ComponentRemoved(entity.ToEntity(this), e.Component));
                    return true;
                }
                return false;
            });
        }

        internal object GetEntityComponent(Guid entity, Type componentType)
        {
            return _components.Where(e => e.Type == componentType).FirstOrDefault();
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

        public void RegisterComponent(Type componentType, Func<object> factory)
            => _componentFactories[componentType] = factory;

        public void RegisterEntity(string entityName, ICollection<Type> components)
            => _entityDefinitions[entityName] = components;

        public void RegisterSystem(IEntitySystem system)
        {
            _systems.Add(system);
        }

        #endregion

        #region DrawableGameComponent Methods

        public override void Initialize() => ForEachSystem(s => s.Initialize(Game));

        protected override void LoadContent() => ForEachSystem(s => s.LoadContent(Game.Content));
        protected override void UnloadContent() => ForEachSystem(s => s.UnloadContent());

        public override void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
            {
                foreach (var entity in _entities)
                    system.Update(entity.ToEntity(this), gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var system in _systems)
            {
                foreach (var entity in _entities)
                    system.Draw(entity.ToEntity(this), gameTime);
            }
        }

        #endregion

        private void ForEachSystem(Action<IEntitySystem> action)
        {
            foreach (var system in _systems)
                action(system);
        }

        private struct EntityComponent
        {
            public Guid Entity;
            public Type Type;
            public object Component;
        }
    }

    static class GuidExtensions
    {
        internal static Entity ToEntity(this Guid guid, EntityComponentSystem ecs) => new Entity(ecs, guid);
    }
}