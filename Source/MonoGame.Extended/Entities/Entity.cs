using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class Entity
    {
        private readonly List<EntityComponent> _components;
        private readonly HashSet<EntitySystem> _systems;

        private readonly TransformComponent _transform;

        public Entity(long id, string name)
        {
            _components = new List<EntityComponent>();
            _systems = new HashSet<EntitySystem>();

            _transform = new TransformComponent();
            _transform.Entity = this;

            _components.Add(_transform);

            Id = id;
            Name = name;
        }

        public Entity Parent
        {
            get { return _transform.Parent; }
            set { _transform.Parent = value; }
        }

        public long Id { get; }
        public string Name { get; }
        public object Tag { get; set; }

        public TransformComponent Transform => _transform;

        public override string ToString()
        {
            return Name;
        }

        public void AttachComponent(EntityComponent component)
        {
            if (component is TransformComponent)
                throw new ArgumentException($"Cannot attach {typeof(TransformComponent)}");

            if (component.Entity != null)
                throw new InvalidOperationException("Component already attached to another entity");

            component.Entity = this;
            _components.Add(component);
        }

        public void DetachComponent(EntityComponent component)
        {
            if (component is TransformComponent)
                throw new ArgumentException($"Cannot detach {typeof(TransformComponent)}");

            if (component.Entity != this)
                throw new InvalidOperationException("Component not attached to entity");

            component.Entity = null;
            _components.Remove(component);
        }

        protected void AttachSystem(EntitySystem system)
        {
            if (system.Entity != null)
                throw new ArgumentException($"{system.GetType()} is already attached to another entity");

            if (_systems.Contains(system))
                throw new InvalidOperationException($"{system.GetType()} is already attached to entity");

            system.Initialize(this);
        }

        internal void DetachSystem(EntitySystem system)
        {
            if (system.Entity != this)
                throw new InvalidOperationException("System not attached to entity");

            _systems.Remove(system);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Draw(gameTime);
        }

        public void Destroy(float delaySeconds = 0f)
        {
            //_systems.DestroyEntity(this, delaySeconds);
            throw new NotImplementedException();
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return GetComponents<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>() where T : EntityComponent
        {
            return _components.OfType<T>();
        }
    }
}