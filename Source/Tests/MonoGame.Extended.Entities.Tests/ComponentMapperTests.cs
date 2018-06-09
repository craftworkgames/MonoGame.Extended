using System;
using MonoGame.Extended.Entities.Systems;
using Xunit;

namespace MonoGame.Extended.Entities.Tests
{
    public class ComponentManagerTests
    {
        [Fact]
        public void CreateComponentManager()
        {
            var manager = new ComponentManager();
            throw new NotImplementedException();
        }
    }

    public class ComponentMapperTests
    {
        [Fact]
        public void CreateComponentMapper()
        {
            var mapper = new ComponentMapper<object>(0);

            Assert.Equal(typeof(object), mapper.ComponentType);
            Assert.Empty(mapper.Components);
        }

        [Fact]
        public void PutAndGetComponent()
        {
            const int entityId = 3;

            var mapper = new ComponentMapper<Transform2>(1);
            var component = new Transform2();

            mapper.CreateComponent(entityId, component);

            Assert.Equal(typeof(Transform2), mapper.ComponentType);
            Assert.True(mapper.Components.Count >= 1);
            Assert.Same(component, mapper.GetComponent(entityId));
        }

        [Fact]
        public void DeleteComponent()
        {
            const int entityId = 1;

            var mapper = new ComponentMapper<Transform2>(2);
            var component = new Transform2();

            mapper.CreateComponent(entityId, component);
            mapper.DeleteComponent(entityId);

            Assert.False(mapper.HasComponent(entityId));
        }

        [Fact]
        public void HasComponent()
        {
            const int entityId = 0;

            var mapper = new ComponentMapper<Transform2>(3);
            var component = new Transform2();

            Assert.False(mapper.HasComponent(entityId));

            mapper.CreateComponent(entityId, component);

            Assert.True(mapper.HasComponent(entityId));
        }
    }
}