using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities
{
    public class Entity : Transform2D<Entity>
    {
        private readonly List<EntityComponent> _components;
        private readonly EntityComponentSystem _entityComponentSystem;

        public Entity(EntityComponentSystem entityComponentSystem, long id, string name)
        {
            _entityComponentSystem = entityComponentSystem;
            _components = new List<EntityComponent>();

            Id = id;
            Name = name;
            Position = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
        }

        public long Id { get; }
        public string Name { get; }
        public object Tag { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void AttachComponent(EntityComponent component)
        {
            if (component.Entity != null)
                throw new InvalidOperationException("Component already attached to another entity");

            component.Entity = this;

            _components.Add(component);
            _entityComponentSystem.AttachComponent(component);
        }

        public void DetachComponent(EntityComponent component)
        {
            if (component.Entity != this)
                throw new InvalidOperationException("Component not attached to entity");

            component.Entity = null;
            _components.Remove(component);
            _entityComponentSystem.DetachComponent(component);
        }

        public void Destroy(float delaySeconds = 0f)
        {
            _entityComponentSystem.DestroyEntity(this, delaySeconds);
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return GetComponents<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>()
        {
            return _components.OfType<T>();
        }
    }
}