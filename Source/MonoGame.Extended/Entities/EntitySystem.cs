using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace MonoGame.Extended.Entities
{
    public abstract class EntitySystem
    {
        internal EntityComponentSystem Ecs { get; set; }

        protected virtual void LoadContent(ContentManager contentManager) { }
        protected virtual void UnloadContent() { }

        protected virtual void Update(Entity entity, GameTime gameTime) { }
        protected virtual void Draw(Entity entity, GameTime gameTime) { }

        #region Entity Events

        protected void CreateEntity(string entityName, Action<Entity> initializer) => Ecs.CreateEntity(entityName, initializer);

        protected virtual void EntityCreated(Entity entity) { }
        protected virtual void EntityRemoved(Entity entity) { }

        protected virtual void ComponentAdded(Entity entity, object component) { }
        protected virtual void ComponentRemoved(Entity entity, object component) { }

        #endregion

        #region Internal Proxy Methods

        internal void LoadContentInternal(ContentManager contentManager) => LoadContent(contentManager);
        internal void UnloadContentInternal() => UnloadContent();

        internal void UpdateInternal(Entity entity, GameTime gameTime) => Update(entity, gameTime);
        internal void DrawInternal(Entity entity, GameTime gameTime) => Draw(entity, gameTime);

        internal void EntityCreatedInternal(Entity entity) => EntityCreated(entity);
        internal void EntityRemovedInternal(Entity entity) => EntityRemoved(entity);

        internal void ComponentAddedInternal(Entity entity, object component) => ComponentAdded(entity, component);
        internal void ComponentRemovedInternal(Entity entity, object component) => ComponentRemoved(entity, component);

        #endregion
    }
}