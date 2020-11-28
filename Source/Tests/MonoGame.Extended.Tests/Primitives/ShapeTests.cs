using Xunit;

namespace MonoGame.Extended.Tests.Primitives
{
    public class ShapeTests
    {
        [Fact]
        public void CircCircIntersectionSameCircleTest()
        {
            var shape1 = new CircleF(Point2.Zero, 2.0f);
            var shape2 = new CircleF(Point2.Zero, 2.0f);

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void CircCircIntersectionOverlappingTest()
        {
            var shape1 = new CircleF(new Point2(1, 2), 2.0f);
            var shape2 = new CircleF(Point2.Zero, 2.0f);

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void CircleCircleNotIntersectingTest()
        {
            var shape1 = new CircleF(new Point2(5, 5), 2.0f);
            var shape2 = new CircleF(Point2.Zero, 2.0f);

            Assert.False(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectRectSameRectTest()
        {
            var shape1 = new RectangleF(Point2.Zero, new Size2(5, 5));
            var shape2 = new RectangleF(Point2.Zero, new Size2(5, 5));

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectRectIntersectingTest()
        {
            var shape1 = new RectangleF(Point2.Zero, new Size2(5, 5));
            var shape2 = new RectangleF(new Point2(3, 3), new Size2(5, 5));

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectRectNotIntersectingTest()
        {
            var shape1 = new RectangleF(Point2.Zero, new Size2(5, 5));
            var shape2 = new RectangleF(new Point2(10, 10), new Size2(5, 5));

            Assert.False(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectCircContainedTest()
        {
            var shape1 = new RectangleF(Point2.Zero, new Size2(5, 5));
            var shape2 = new CircleF(Point2.Zero, 4);

            Assert.True(shape1.Intersects(shape2));
            Assert.True(shape2.Intersects(shape1));
        }


        [Fact]
        public void RectCircOnEdgeTest()
        {
            var shape1 = new RectangleF(Point2.Zero, new Size2(5, 5));
            var shape2 = new CircleF(new Point2(5, 0), 4);

            Assert.True(shape1.Intersects(shape2));
            Assert.True(shape2.Intersects(shape1));
        }
    }
}
