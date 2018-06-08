using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Legacy;

namespace MonoGame.Extended.Gui.Tests.Implementation
{
    [EntityTemplate(Name)]
    public class EntityTemplateUsingManager : EntityTemplate
    {
        public const string Name = nameof(EntityTemplateUsingManager);

        protected override void Build(Entity entity)
        {
            var num = Manager.EntityManager.TotalEntitiesCount;
            entity.Attach<EntityComponentBasic>(c => c.Number = num);
        }
    }
}