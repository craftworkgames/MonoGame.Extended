using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;

namespace Demo.Platformer.Entities.Factories
{
    public interface ITiledEntityFactory
    {
        void BuildEntity(Entity entity, TiledMapObject mapObject);
        void LoadContent(ContentManager contentManager);
    }
}
