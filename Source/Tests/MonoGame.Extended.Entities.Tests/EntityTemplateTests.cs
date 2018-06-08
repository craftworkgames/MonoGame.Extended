using MonoGame.Extended.Entities.Legacy;
using MonoGame.Extended.Gui.Tests.Implementation;
using Xunit;

namespace MonoGame.Extended.Entities.Tests
{
    public class EntityTemplateTests
    {
        [Fact]
        public void ECS_CreateEntityFromTemplate_Basic_Test()
        {
            // Seems to work with null.
            var ecs = new EntityComponentSystem(null);
            ecs.Scan(typeof(EntityTemplateBasic).Assembly);
            var manager = ecs.EntityManager;
            var entity = manager.CreateEntityFromTemplate(EntityTemplateBasic.Name);

            Assert.NotNull(entity);
            Assert.NotNull(entity.Get<EntityComponentBasic>());
        }

        [Fact]
        public void ECS_CreateEntityFromTemplate_UsingManager_Test()
        {
            // Seems to work with null.
            var ecs = new EntityComponentSystem(null);
            ecs.Scan(typeof(EntityTemplateUsingManager).Assembly);
            var manager = ecs.EntityManager;
            var entity = manager.CreateEntityFromTemplate(EntityTemplateUsingManager.Name);

            Assert.NotNull(entity);
            Assert.NotNull(entity.Get<EntityComponentBasic>());
        }
    }
}