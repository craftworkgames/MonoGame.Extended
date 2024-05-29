using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests.Primitives
{
    public class RectangleFTests
    {
        [Fact]
        public void Rectangle_Intersects_Test()
        {
            var rect1 = new Rectangle(0, 0, 32, 32);
            var rect2 = new Rectangle(32, 32, 32, 32);

            Assert.False(rect1.Intersects(rect2));
        }

        [Fact]
        public void PassVector2AsConstructorParameter_Test()
        {
            var rect1 = new RectangleF(new Vector2(0, 0), new SizeF(12.34f, 56.78f));
            var rect2 = new RectangleF(new Vector2(0, 0), new Vector2(12.34f, 56.78f));

            Assert.Equal(rect1, rect2);
        }

        [Fact]
        public void PassPointAsConstructorParameter_Test()
        {
            var rect1 = new RectangleF(new Vector2(0, 0), new SizeF(12, 56));
            var rect2 = new RectangleF(new Vector2(0, 0), new SizeF(12, 56));

            Assert.Equal(rect1, rect2);
        }

        public class Transform
        {
            [Fact]
            public void Center_point_is_not_translated()
            {
                var rectangle = new RectangleF(new Vector2(0, 0), new SizeF(20, 30));
                var transform = Matrix3x2.Identity;

                var result = RectangleF.Transform(rectangle, ref transform);

                Assert.Equal(new Vector2(10, 15), result.Center);
            }

            [Fact]
            public void Center_point_is_translated()
            {
                var rectangleF = new RectangleF(new Vector2(0, 0), new SizeF(20, 30));
                var transform = Matrix3x2.CreateTranslation(1, 2);

                var result = RectangleF.Transform(rectangleF, ref transform);

                Assert.Equal(new Vector2(11, 17), result.Center);
            }

            [Fact]
            public void Size_is_not_changed_by_identity_transform()
            {
                var rectangle = new RectangleF(new Vector2(0, 0), new SizeF(20, 30));
                var transform = Matrix3x2.Identity;

                var result = RectangleF.Transform(rectangle, ref transform);

                Assert.Equal(new SizeF(20, 30), result.Size);
            }

            [Fact]
            public void Size_is_not_changed_by_translation()
            {
                var rectangle = new RectangleF(new Vector2(0, 0), new SizeF(20, 30));
                var transform = Matrix3x2.CreateTranslation(1, 2);

                var result = RectangleF.Transform(rectangle, ref transform);

                Assert.Equal(new SizeF(20, 30), result.Size);
            }

            [Fact]
            public void Applies_rotation_and_translation()
            {
                /*   Rectangle with center point aligned in coordinate system with origin 0.
                 *
                 *                    :
                 *                    :
                 *                    +-+
                 *                    | |
                 *                    |p|
                 *                    | |
                 *     ...............0-+............
                 *                    :
                 *                    :
                 *                    :
                 *
                 *    Rotate center point p, 90 degrees around origin 0.
                 *
                 *                    :
                 *                    :
                 *                +---+
                 *                | p |
                 *     ...........+---0..............
                 *                    :
                 *                    :
                 *                    :
                 *
                 *    Then translate rectangle by x=10 and y=20.
                 *                    :
                 *                    :     +---+
                 *                    :     | p |
                 *    y=21 - - - - - - - -> +---+
                 *                    .
                 *                    :
                 *    ...............0..............
                 *                    :
                 *                    :
                 *                    :
                 */
                var rectangle = new RectangleF(new Vector2(0, 0), new SizeF(2, 4));
                var transform =
                    Matrix3x2.CreateRotationZ(MathHelper.PiOver2)
                    *
                    Matrix3x2.CreateTranslation(10, 20);

                var result = RectangleF.Transform(rectangle, ref transform);

                Assert.Equal(-2 + 10, result.Center.X, 6);
                Assert.Equal(1 + 20, result.Center.Y, 6);
                Assert.Equal(4, result.Size.Width, 6);
                Assert.Equal(2, result.Size.Height, 6);
            }
        }
    }
}
