using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Entities
{
    public class Entity
    {
        private readonly List<EntityComponent> _components;
        private readonly List<EntitySystem> _systems;

        private readonly Transform2DComponent _transform;

        private bool _initialized;

        public Entity()
        {
            _components = new List<EntityComponent>();
            _systems = new List<EntitySystem>();

            _transform = new Transform2DComponent();
            _transform.Entity = this;

            _components.Add(_transform);
        }

        public Entity Parent
        {
            get { return _transform.Parent; }
            set { _transform.Parent = value; }
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public object Tag { get; set; }

        public IReadOnlyCollection<EntityComponent> Components => new ReadOnlyCollection<EntityComponent>(_components);
        public Transform2DComponent Transform => _transform;

        public override string ToString()
        {
            return Name;
        }

        public EntityComponent AttachComponent(EntityComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is Transform2DComponent)
                throw new ArgumentException($"Cannot attach {typeof(Transform2DComponent)}");

            if (component.Entity != null)
                throw new InvalidOperationException("Component already attached to another entity");

            if (_components.Contains(component))
                throw new InvalidOperationException("Component already attached to entity");

            component.Entity = this;
            _components.Add(component);

            return component;
        }

        public void DetachComponent(EntityComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is Transform2DComponent)
                throw new ArgumentException($"Cannot detach {typeof(Transform2DComponent)}");

            if (component.Entity != this || !_components.Contains(component))
                throw new InvalidOperationException("Component not attached to entity");

            component.Entity = null;
            _components.Remove(component);
        }

        public void DisposeComponent(EntityComponent component)
        {
            DetachComponent(component);
            component.Dispose();
        }

        public EntitySystem AttachSystem(EntitySystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (system.Entity != null)
                throw new ArgumentException($"{system.GetType()} is already attached to another entity");

            if (_systems.Contains(system))
                throw new InvalidOperationException($"{system.GetType()} is already attached to entity");

            if (_initialized)
                system.Initialize(this);

            _systems.Add(system);
            return system;
        }

        internal void DetachSystem(EntitySystem system)
        {
            if (system.Entity != this || !_systems.Contains(system))
                throw new InvalidOperationException("System not attached to entity");

            _systems.Remove(system);
        }

        public void Initialize()
        {
            if (_initialized)
                throw new Exception("Entity already initialized");

            foreach (var system in _systems)
                system.Initialize(this);

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Update(gameTime);
            OnUpdate(gameTime);
        }

        protected virtual void OnUpdate(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Draw(gameTime);
            OnDraw(gameTime);
        }

        protected virtual void OnDraw(GameTime gameTime) { }

        public void Destroy(float delaySeconds = 0f)
        {
            //_systems.DestroyEntity(this, delaySeconds);
            throw new NotImplementedException();
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return (T)_components.Find(e => e.GetType() == typeof(T));
        }
    }
}