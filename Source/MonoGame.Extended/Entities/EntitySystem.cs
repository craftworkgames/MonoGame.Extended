using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Entities
{
    public abstract class EntitySystem : IEntitySystem
    {
        public virtual void LoadContent(ContentManager contentManager) { }
        public virtual void UnloadContent() { }

        public virtual void Update(Entity entity, GameTime gameTime) { }
        public virtual void Draw(Entity entity, GameTime gameTime) { }

        #region Entity Events

        public virtual void EntityCreated(Entity entity) { }
        public virtual void EntityRemoved(Entity entity) { }

        public virtual void ComponentAdded(Entity entity, object component) { }
        public virtual void ComponentRemoved(Entity entity, object component) { }

        #endregion
    }
}