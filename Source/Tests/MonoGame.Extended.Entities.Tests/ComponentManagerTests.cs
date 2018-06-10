using MonoGame.Extended.Sprites;
using Xunit;

namespace MonoGame.Extended.Entities.Tests
{
    public class ComponentManagerTests
    {
        [Fact]
        public void GetMapperForType()
        {
            var componentManager = new ComponentManager();
            var transformMapper = componentManager.GetMapper<Transform2>();
            var spriteMapper = componentManager.GetMapper<Sprite>();

            Assert.IsType<ComponentMapper<Transform2>>(transformMapper);
            Assert.IsType<ComponentMapper<Sprite>>(spriteMapper);
            Assert.Equal(0, transformMapper.Id);
            Assert.Equal(1, spriteMapper.Id);
            Assert.Same(spriteMapper, componentManager.GetMapper<Sprite>());
        }

        [Fact]
        public void GetComponentTypeId()
        {
            var componentManager = new ComponentManager();

            Assert.Equal(0, componentManager.GetComponentTypeId(typeof(Transform2)));
            Assert.Equal(1, componentManager.GetComponentTypeId(typeof(Sprite)));
            Assert.Equal(0, componentManager.GetComponentTypeId(typeof(Transform2)));
        }
    }
}