using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests.Primitives;

public class ShapeTests
{
    public class CircleFTests
    {
        [Fact]
        public void CircCircIntersectionSameCircleTest()
        {
            IShapeF shape1 = new CircleF(Vector2.Zero, 2.0f);
            IShapeF shape2 = new CircleF(Vector2.Zero, 2.0f);

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void CircCircIntersectionOverlappingTest()
        {
            IShapeF shape1 = new CircleF(new Vector2(1, 2), 2.0f);
            IShapeF shape2 = new CircleF(Vector2.Zero, 2.0f);

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void CircleCircleNotIntersectingTest()
        {
            IShapeF shape1 = new CircleF(new Vector2(5, 5), 2.0f);
            IShapeF shape2 = new CircleF(Vector2.Zero, 2.0f);

            Assert.False(shape1.Intersects(shape2));
        }
    }

    public class RectangleFTests
    {
        [Fact]
        public void RectRectSameRectTest()
        {
            IShapeF shape1 = new RectangleF(Vector2.Zero, new SizeF(5, 5));
            IShapeF shape2 = new RectangleF(Vector2.Zero, new SizeF(5, 5));

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectRectIntersectingTest()
        {
            IShapeF shape1 = new RectangleF(Vector2.Zero, new SizeF(5, 5));
            IShapeF shape2 = new RectangleF(new Vector2(3, 3), new SizeF(5, 5));

            Assert.True(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectRectNotIntersectingTest()
        {
            IShapeF shape1 = new RectangleF(Vector2.Zero, new SizeF(5, 5));
            IShapeF shape2 = new RectangleF(new Vector2(10, 10), new SizeF(5, 5));

            Assert.False(shape1.Intersects(shape2));
        }

        [Fact]
        public void RectCircContainedTest()
        {
            IShapeF shape1 = new RectangleF(Vector2.Zero, new SizeF(5, 5));
            IShapeF shape2 = new CircleF(Vector2.Zero, 4);

            Assert.True(shape1.Intersects(shape2));
            Assert.True(shape2.Intersects(shape1));
        }

        [Fact]
        public void RectCircOnEdgeTest()
        {
            IShapeF shape1 = new RectangleF(Vector2.Zero, new SizeF(5, 5));
            IShapeF shape2 = new CircleF(new Vector2(5, 0), 4);

            Assert.True(shape1.Intersects(shape2));
            Assert.True(shape2.Intersects(shape1));
        }
    }

    public class OrientedRectangleTests
    {
        [Fact]
        public void Axis_aligned_rectangle_intersects_circle()
        {
            /*
             *                    :
             *                    :
             *                   +*+
             *        ...........* *.........
             *                   +*+
             *                    :
             *                    :
             */
            IShapeF rectangle = new OrientedRectangle(Vector2.Zero, new SizeF(1, 1), Matrix3x2.Identity);
            var circle = new CircleF(Vector2.Zero, 1);

            Assert.True(rectangle.Intersects(circle));
        }

        [Fact]
        public void Axis_aligned_rectangle_does_not_intersect_circle_in_top_left()
        {
            /*
             *                  * :
             *                 * *:
             *                  *+-+
             *        ...........| |.........
             *                   +-+
             *                    :
             *                    :
             */
            IShapeF rectangle = new OrientedRectangle(Vector2.Zero, new SizeF(1, 1), Matrix3x2.Identity);
            var circle = new CircleF(new Vector2(-2, 1), .99f);

            Assert.False(rectangle.Intersects(circle));
        }

        [Fact]
        public void Rectangle_rotated_45_degrees_does_not_intersect_circle_in_bottom_right()
        {
            /*
             *                    :
             *                    :
             *                    +-.
             *        .........../ / ........
             *                  +./* *
             *                    *   *
             *                    :* *
             */
            IShapeF rectangle = new OrientedRectangle(new Vector2(-1, 1), new SizeF(1.42f, 1.42f), Matrix3x2.CreateRotationZ(-MathHelper.PiOver4));
            var circle = new CircleF(new Vector2(1, -1), 1.4f);

            Assert.False(rectangle.Intersects(circle));
        }

        [Fact]
        public void Axis_aligned_rectangle_does_not_intersect_rectangle()
        {
            /*
             *                    :
             *                    :
             *                  +-+
             *        ..........| |**.......
             *                  +-+ *
             *                    :**
             *                    :
             */
            IShapeF rectangle = new OrientedRectangle(new Vector2(-1, 0), new SizeF(1, 1), Matrix3x2.Identity);
            var rect = new RectangleF(new Vector2(0.001f, 0), new SizeF(2, 2));

            Assert.False(rectangle.Intersects(rect));
        }

        [Fact]
        public void Axis_aligned_rectangle_intersects_rectangle()
        {
            /*
             *                    :
             *                    :
             *                  +-+
             *        ..........| |**.......
             *                  +-+ *
             *                    :**
             *                    :
             */
            IShapeF rectangle = new OrientedRectangle(new Vector2(-1, 0), new SizeF(1, 1), Matrix3x2.Identity);
            var rect = new RectangleF(new Vector2(0, 0), new SizeF(2, 2));

            Assert.True(rectangle.Intersects(rect));
        }
    }
}
