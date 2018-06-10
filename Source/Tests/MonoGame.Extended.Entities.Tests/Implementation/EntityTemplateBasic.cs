using MonoGame.Extended.Entities.Legacy;

namespace MonoGame.Extended.Gui.Tests.Implementation
{
    [EntityTemplate(Name)]
    public class EntityTemplateBasic : EntityTemplate
    {
        public const string Name = nameof(EntityTemplateBasic);

        protected override void Build(Entity entity)
        {
            entity.Attach<EntityComponentBasic>();
        }
    }
}