using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public class RectangleExtensionsTests
    {
        [Fact]
        public void Clip_ReturnsIntersectionRectangle()
        {
            var rect = new Rectangle(0, 0, 10, 10);
            var clip1 = new Rectangle(2, 2, 5, 5);
            var clip2 = new Rectangle(2, 2, 15, 15);
            var clip3 = new Rectangle(-2, -2, 5, 5);

            Assert.Equal(new Rectangle(2, 2, 5, 5), rect.Clip(clip1));
            Assert.Equal(new Rectangle(2, 2, 8, 8), rect.Clip(clip2));
            Assert.Equal(new Rectangle(0, 0, 3, 3), rect.Clip(clip3));
        }
    }
}
