using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Demo.Platformer.Entities
{
    public class Entity : IMovable, IRotatable, IScalable
    {
        private readonly IEntityManager _entityManager;

        public Entity(IEntityManager entityManager, long id)
        {
            _entityManager = entityManager;
            Id = id;
            Scale = Vector2.One;
        }

        public long Id { get; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public void AttachComponent(EntityComponent component)
        {
            component.Entity = this;
            _entityManager.AttachComponent(component);
        }
    }
}
