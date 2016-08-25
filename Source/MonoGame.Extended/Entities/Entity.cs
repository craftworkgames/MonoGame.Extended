using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities
{
    public class Entity : Transform2D<Entity>//, IEquatable<Entity>
    {
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

        private readonly List<EntityComponent> _components;

        //public static bool operator ==(Entity a, Entity b)
        //{
        //    if (ReferenceEquals(a, null)) return false;
        //    if (ReferenceEquals(b, null)) return false;
        //    return a.Id == b.Id;
        //}

        //public static bool operator !=(Entity a, Entity b)
        //{
        //    if (ReferenceEquals(a, null)) return false;
        //    if (ReferenceEquals(b, null)) return false;
        //    return a.Id != b.Id;
        //}

        //public bool Equals(Entity other)
        //{
        //    if (ReferenceEquals(other, null)) return false;
        //    return Id == other.Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    var other = obj as Entity;
        //    return !ReferenceEquals(other, null) && Equals(other);
        //}

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}

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

        public void Destroy()
        {
            _entityComponentSystem.DestroyEntity(this);
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return _components.OfType<T>().FirstOrDefault();
        }
    }
}
