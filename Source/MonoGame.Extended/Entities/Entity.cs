using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities
{
    public class Entity : Transform2D<Entity>
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

        public override string ToString()
        {
            return Name;
        }

        public void AttachComponent(EntityComponent component)
        {
            _entityComponentSystem.AttachComponent(this, component);
        }

        public void DetachComponent(EntityComponent component)
        {
            _entityComponentSystem.DetachComponent(this, component);
        }

        public void Destroy()
        {
            _entityComponentSystem.DestroyEntity(this);
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return _entityComponentSystem.GetComponent<T>(this);
        }
    }
}
