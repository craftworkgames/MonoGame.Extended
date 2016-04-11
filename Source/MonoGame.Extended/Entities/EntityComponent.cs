using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    public abstract class EntityComponent : IUpdate
    {
        public string Name { get; set; }
        internal Entity Entity;
        internal void SetEntity(Entity entity) {
            Entity = entity;
        }
        public virtual void Update(GameTime gameTime) { }

    }
}