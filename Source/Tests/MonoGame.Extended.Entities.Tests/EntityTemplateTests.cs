using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Gui.Tests.Implementation;
using NUnit.Framework;

namespace MonoGame.Extended.Gui.Tests
{
    [TestFixture]
    public class EntityTemplateTests
    {
        [Test]
        public void ECS_CreateEntityFromTemplate_Basic_Test()
        {
            // Seems to work with null.
            var ecs = new EntityComponentSystem(null);
            ecs.Scan(typeof(EntityTemplateBasic).Assembly);
            var manager = ecs.EntityManager;
            Entity entity = null;
            try
            {
                entity = manager.CreateEntityFromTemplate(EntityTemplateBasic.Name);
            }
            catch (Exception e)
            {
                Assert.Fail("Failed to create entity from template.\n" + e.StackTrace);
            }
            
            Assert.NotNull(entity);
            Assert.NotNull(entity.Get<EntityComponentBasic>());
        }

        [Test]
        public void ECS_CreateEntityFromTemplate_UsingManager_Test()
        {
            // Seems to work with null.
            var ecs = new EntityComponentSystem(null);
            ecs.Scan(typeof(EntityTemplateUsingManager).Assembly);
            var manager = ecs.EntityManager;
            Entity entity = null;
            try
            {
                entity = manager.CreateEntityFromTemplate(EntityTemplateUsingManager.Name);
            }
            catch (Exception e)
            {
                Assert.Fail("Failed to create entity from template.\n" + e.StackTrace);
            }

            Assert.NotNull(entity);
            Assert.NotNull(entity.Get<EntityComponentBasic>());
        }
    }
}