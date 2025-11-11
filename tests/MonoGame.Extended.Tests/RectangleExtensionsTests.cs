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

        [Fact]
        public void Normalize_WithNegativeWidthAndHeight_AdjustsPositionAndMakesDimensionsPositive()
        {
            Rectangle rectangle = new Rectangle(32, 32, -32, -32);
            Rectangle actual = RectangleExtensions.Normalize(rectangle);

            Rectangle expected = new Rectangle(0, 0, 32, 32);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize_WithNegativeWidthOnly_AdjustsXAndMakesWidthPositive()
        {
            Rectangle rectangle = new Rectangle(100, 50, -20, 30);
            Rectangle actual = RectangleExtensions.Normalize(rectangle);

            Rectangle expected = new Rectangle(80, 50, 20, 30);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize_WithNegativeHeightOnly_AdjustsYAndMakesHeightPositive()
        {
            Rectangle rectangle = new Rectangle(50, 100, 30, -20);
            Rectangle actual = RectangleExtensions.Normalize(rectangle);

            Rectangle expected = new Rectangle(50, 80, 30, 20);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize_WithPositiveDimensions_DoesNotModifyRectangle()
        {
            Rectangle rectangle = new Rectangle(10, 20, 30, 40);
            Rectangle actual = RectangleExtensions.Normalize(rectangle);

            Rectangle expected = new Rectangle(10, 20, 30, 40);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize_StaticMethod_ReturnsNormalizedRectangle()
        {
            Rectangle rectangle = new Rectangle(32, 32, -32, -32);

            Rectangle actual = RectangleExtensions.Normalize(rectangle);
            Rectangle expected = new Rectangle(0, 0, 32, 32);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Normalize_RefOutMethod_OutputsNormalizedRectangle()
        {
            Rectangle rectangle = new Rectangle(32, 32, -32, -32);

            RectangleExtensions.Normalize(ref rectangle, out Rectangle actual);
            Rectangle expected = new Rectangle(0, 0, 32, 32);

            Assert.Equal(expected, actual);
        }
    }
}
