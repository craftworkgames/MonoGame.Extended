using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Entities
{
    public interface IEntitySystem
    {
        void Initialize(Game game);

        void LoadContent(ContentManager contentManager);
        void UnloadContent();

        void Update(Entity entity, GameTime gameTime);
        void Draw(Entity entity, GameTime gameTime);

        #region Entity Events

        void EntityCreated(Entity entity);
        void EntityRemoved(Entity entity);

        void ComponentAdded(Entity entity, object component);
        void ComponentRemoved(Entity entity, object component);

        #endregion
    }
}
