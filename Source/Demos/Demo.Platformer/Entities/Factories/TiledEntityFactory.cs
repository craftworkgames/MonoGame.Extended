using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using Demo.Platformer.Entities.Components;

namespace Demo.Platformer.Entities.Factories
{
    public class TiledEntityFactory : ITiledEntityFactory
    {
        public virtual void BuildEntity(Entity entity, TiledMapObject mapObject)
        {
            var collisionBody = entity.GetComponent<CollisionBody>();
            collisionBody.Size = mapObject.Size;
            collisionBody.Position = mapObject.Position;
        }

        public virtual void LoadContent(ContentManager contentManager) { }
    }
}
