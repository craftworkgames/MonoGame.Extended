using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Demo.Platformer.Entities
{
    public class Entity : IMovable, IRotatable, IScalable
    {
        private readonly EntityComponentSystem _entityComponentSystem;

        public Entity(EntityComponentSystem entityComponentSystem, long id, string name)
        {
            _entityComponentSystem = entityComponentSystem;

            Id = id;
            Name = name;
            Position = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
        }

        public long Id { get; }
        public string Name { get; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void AttachComponent(EntityComponent component)
        {
            if (component.Entity != null)
                throw new InvalidOperationException("Component already attached to another entity");

            component.Entity = this;
            _entityComponentSystem.AttachComponent(component);
        }

        public void DetachComponent(EntityComponent component)
        {
            if (component.Entity != this)
                throw new InvalidOperationException("Component not attached to entity");

            component.Entity = null;
            _entityComponentSystem.DetachComponent(component);
        }

        public void Destroy()
        {
            _entityComponentSystem.DestroyEntity(this);
        }
    }
}
