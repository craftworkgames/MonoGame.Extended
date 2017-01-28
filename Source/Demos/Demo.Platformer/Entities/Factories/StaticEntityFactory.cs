using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using Demo.Platformer.Entities.Components;

namespace Demo.Platformer.Entities.Factories
{
    public sealed class StaticEntityFactory : TiledEntityFactory
    {
        public override void BuildEntity(Entity entity, TiledMapObject mapObject)
        {
            entity.GetComponent<CollisionBody>().IsStatic = true;
            base.BuildEntity(entity, mapObject);
        }
    }
}
