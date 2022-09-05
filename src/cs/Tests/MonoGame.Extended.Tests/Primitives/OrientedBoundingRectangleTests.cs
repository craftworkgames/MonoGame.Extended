using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Xunit;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoGame.Extended.Tests.Primitives;

public class OrientedBoundingRectangleTests
{
    [Fact]
    public void Initializes_oriented_bounding_rectangle()
    {
        var rect = new OrientedBoundingRectangle(new Point2(1, 2), new Size2(3, 4), new Matrix2(5, 6, 7, 8, 9, 10));

        Assert.Equal(new Point2(1, 2), rect.Center);
        Assert.Equal(new Vector2(3, 4), rect.Radii);
        Assert.Equal(new Matrix2(5, 6, 7, 8, 9, 10), rect.Orientation);
    }

    public static readonly IEnumerable<object[]> _equalsComparisons = new[]
        {
            new object[]
                {
                    "empty compared with empty is true",
                    new OrientedBoundingRectangle(Point2.Zero, Size2.Empty, Matrix2.Identity),
                    new OrientedBoundingRectangle(Point2.Zero, Size2.Empty, Matrix2.Identity)
                },
            new object[]
                {
                    "initialized compared with initialized true",
                    new OrientedBoundingRectangle(new Point2(1, 2), new Size2(3, 4), new Matrix2(5, 6, 7, 8, 9, 10)),
                    new OrientedBoundingRectangle(new Point2(1, 2), new Size2(3, 4), new Matrix2(5, 6, 7, 8, 9, 10))
                }
        };

    [Theory]
    [MemberData(nameof(_equalsComparisons))]
#pragma warning disable xUnit1026
    public void Equals_comparison(string name, OrientedBoundingRectangle first, OrientedBoundingRectangle second)
#pragma warning restore xUnit1026
    {
        Assert.True(first == second);
        Assert.False(first != second);
    }

    public class Transform
    {
        [Fact]
        public void Center_point_is_not_translated()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(1, 2), new Size2(), new Matrix2());
            var transform = Matrix2.Identity;

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Point2(1, 2), result.Center);
        }

        [Fact]
        public void Center_point_is_translated()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(0, 0), new Size2(), new Matrix2());
            var transform = Matrix2.CreateTranslation(1, 2);

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Point2(1, 2), result.Center);
        }

        [Fact]
        public void Radii_is_not_changed_by_identity_transform()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(), new Size2(10, 20), new Matrix2());
            var transform = Matrix2.Identity;

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(10, 20), result.Radii);
        }

        [Fact]
        public void Radii_is_not_changed_by_translation()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(1, 2), new Size2(10, 20), new Matrix2());
            var transform = Matrix2.CreateTranslation(1, 2);

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(10, 20), result.Radii);
        }

        [Fact]
        public void Rotate_45_degrees_left()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(), new Size2(), Matrix2.Identity);
            var transform = Matrix2.CreateRotationZ(MathHelper.PiOver4);

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Point2(), result.Center);
            Assert.Equal(new Vector2(), result.Radii);
            Assert.Equal(Matrix2.CreateRotationZ(MathHelper.PiOver4), result.Orientation);
        }

        [Fact]
        public void Rotate_to_45_degrees_from_180()
        {
            var rectangle = new OrientedBoundingRectangle(new Point2(), new Size2(), Matrix2.CreateRotationZ(MathHelper.Pi));
            var transform = Matrix2.CreateRotationZ(-3 * MathHelper.PiOver4);
            var expectedOrientation = Matrix2.CreateRotationZ(MathHelper.PiOver4);

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Point2(), result.Center);
            Assert.Equal(new Vector2(), result.Radii);
            Assert.Equal(expectedOrientation.M11, result.Orientation.M11, 6);
            Assert.Equal(expectedOrientation.M12, result.Orientation.M12, 6);
            Assert.Equal(expectedOrientation.M21, result.Orientation.M21, 6);
            Assert.Equal(expectedOrientation.M22, result.Orientation.M22, 6);
            Assert.Equal(expectedOrientation.M31, result.Orientation.M31, 6);
            Assert.Equal(expectedOrientation.M32, result.Orientation.M32, 6);
        }

        [Fact]
        public void Applies_rotation_and_translation()
        {
            /*   Rectangle with center point p, aligned in coordinate system with origin 0.
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
             *    Rotate around center point p, 90 degrees around origin 0.
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
            var rectangle = new OrientedBoundingRectangle(new Point2(1, 2), new Size2(2, 4), Matrix2.Identity);
            var transform =
                Matrix2.CreateRotationZ(MathHelper.PiOver2)
                *
                Matrix2.CreateTranslation(10, 20);

            var result = OrientedBoundingRectangle.Transform(rectangle, ref transform);

            Assert.Equal(-2 + 10, result.Center.X, 6);
            Assert.Equal(1 + 20, result.Center.Y, 6);
            Assert.Equal(4, result.Radii.X, 6);
            Assert.Equal(2, result.Radii.Y, 6);
            Assert.Equal(Matrix2.CreateRotationZ(MathHelper.PiOver2), result.Orientation);
        }
    }
}
