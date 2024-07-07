using Xunit;

namespace MonoGame.Extended.ECS.Tests
{
    public class ComponentMapperTests
    {
        [Fact]
        public void CreateComponentMapper()
        {
            var mapper = new ComponentMapper<object>(0, _ => {});

            Assert.Equal(typeof(object), mapper.ComponentType);
            Assert.Empty(mapper.Components);
        }

        [Fact]
        public void OnPut()
        {
            const int entityId = 3;

            var mapper = new ComponentMapper<Transform2>(1, _ => { });
            var component = new Transform2();

            mapper.OnPut += (entId) =>
            {
                Assert.Equal(entityId, entId);
                Assert.Same(component, mapper.Get(entityId));
            };

            mapper.Put(entityId, component);
        }

        [Fact]
        public void PutAndGetComponent()
        {
            const int entityId = 3;

            var mapper = new ComponentMapper<Transform2>(1, _ => { });
            var component = new Transform2();

            mapper.Put(entityId, component);

            Assert.Equal(typeof(Transform2), mapper.ComponentType);
            Assert.True(mapper.Components.Count >= 1);
            Assert.Same(component, mapper.Get(entityId));
        }

        [Fact]
        public void OnDelete()
        {
            const int entityId = 1;

            var mapper = new ComponentMapper<Transform2>(2, _ => { });
            var component = new Transform2();

            mapper.OnDelete += (entId) =>
            {
                Assert.Equal(entityId, entId);
                Assert.False(mapper.Has(entityId));
            };

            mapper.Put(entityId, component);
            mapper.Delete(entityId);
        }

        [Fact]
        public void DeleteComponent()
        {
            const int entityId = 1;

            var mapper = new ComponentMapper<Transform2>(2, _ => { });
            var component = new Transform2();

            mapper.Put(entityId, component);
            mapper.Delete(entityId);

            Assert.False(mapper.Has(entityId));
        }

        [Fact]
        public void HasComponent()
        {
            const int entityId = 0;

            var mapper = new ComponentMapper<Transform2>(3, _ => { });
            var component = new Transform2();

            Assert.False(mapper.Has(entityId));

            mapper.Put(entityId, component);

            Assert.True(mapper.Has(entityId));
        }
    }
}
