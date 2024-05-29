using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xunit;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoGame.Extended.Tests.Primitives;

public class OrientedRectangleTests
{
    [Fact]
    public void Initializes_oriented_rectangle()
    {
        var rectangle = new OrientedRectangle(new Vector2(1, 2), new SizeF(3, 4), new Matrix3x2(5, 6, 7, 8, 9, 10));

        Assert.Equal(new Vector2(1, 2), rectangle.Center);
        Assert.Equal(new Vector2(3, 4), rectangle.Radii);
        Assert.Equal(new Matrix3x2(5, 6, 7, 8, 9, 10), rectangle.Orientation);
        CollectionAssert.Equal(
            new List<Vector2>
                {
                    new(-3, -2),
                    new(-33, -38),
                    new(23, 26),
                    new(53, 62)
                },
            rectangle.Points);
    }

    public static readonly IEnumerable<object[]> _equalsComparisons = new[]
        {
            new object[]
                {
                    "empty compared with empty is true",
                    new OrientedRectangle(Vector2.Zero, SizeF.Empty, Matrix3x2.Identity),
                    new OrientedRectangle(Vector2.Zero, SizeF.Empty, Matrix3x2.Identity)
                },
            new object[]
                {
                    "initialized compared with initialized true",
                    new OrientedRectangle(new Vector2(1, 2), new SizeF(3, 4), new Matrix3x2(5, 6, 7, 8, 9, 10)),
                    new OrientedRectangle(new Vector2(1, 2), new SizeF(3, 4), new Matrix3x2(5, 6, 7, 8, 9, 10))
                }
        };

    [Theory]
    [MemberData(nameof(_equalsComparisons))]
#pragma warning disable xUnit1026
    public void Equals_comparison(string name, OrientedRectangle first, OrientedRectangle second)
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
            var rectangle = new OrientedRectangle(new Vector2(1, 2), new SizeF(), Matrix3x2.Identity);
            var transform = Matrix3x2.Identity;

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(1, 2), result.Center);
        }

        [Fact]
        public void Center_point_is_translated()
        {
            var rectangle = new OrientedRectangle(new Vector2(0, 0), new SizeF(), new Matrix3x2());
            var transform = Matrix3x2.CreateTranslation(1, 2);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(1, 2), result.Center);
        }

        [Fact]
        public void Radii_is_not_changed_by_identity_transform()
        {
            var rectangle = new OrientedRectangle(new Vector2(), new SizeF(10, 20), new Matrix3x2());
            var transform = Matrix3x2.Identity;

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(10, 20), result.Radii);
        }

        [Fact]
        public void Radii_is_not_changed_by_translation()
        {
            var rectangle = new OrientedRectangle(new Vector2(1, 2), new SizeF(10, 20), new Matrix3x2());
            var transform = Matrix3x2.CreateTranslation(1, 2);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(10, 20), result.Radii);
        }

        [Fact]
        public void Orientation_is_rotated_45_degrees_left()
        {
            var rectangle = new OrientedRectangle(new Vector2(), new SizeF(), Matrix3x2.Identity);
            var transform = Matrix3x2.CreateRotationZ(MathHelper.PiOver4);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(), result.Center);
            Assert.Equal(new Vector2(), result.Radii);
            Assert.Equal(Matrix3x2.CreateRotationZ(MathHelper.PiOver4), result.Orientation);
        }

        [Fact]
        public void Orientation_is_rotated_to_45_degrees_from_180()
        {
            var rectangle = new OrientedRectangle(new Vector2(), new SizeF(), Matrix3x2.CreateRotationZ(MathHelper.Pi));
            var transform = Matrix3x2.CreateRotationZ(-3 * MathHelper.PiOver4);
            var expectedOrientation = Matrix3x2.CreateRotationZ(MathHelper.PiOver4);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(new Vector2(), result.Center);
            Assert.Equal(new Vector2(), result.Radii);
            Assert.Equal(expectedOrientation.M11, result.Orientation.M11, 6);
            Assert.Equal(expectedOrientation.M12, result.Orientation.M12, 6);
            Assert.Equal(expectedOrientation.M21, result.Orientation.M21, 6);
            Assert.Equal(expectedOrientation.M22, result.Orientation.M22, 6);
            Assert.Equal(expectedOrientation.M31, result.Orientation.M31, 6);
            Assert.Equal(expectedOrientation.M32, result.Orientation.M32, 6);
        }

        [Fact]
        public void Points_are_same_as_center()
        {
            var rectangle = new OrientedRectangle(new Vector2(1, 2), new SizeF(), Matrix3x2.Identity);
            var transform = Matrix3x2.Identity;

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            CollectionAssert.Equal(
                new List<Vector2>
                    {
                        new(1, 2),
                        new(1, 2),
                        new(1, 2),
                        new(1, 2)
                    },
                result.Points);
        }

        [Fact]
        public void Points_are_translated()
        {
            var rectangle = new OrientedRectangle(new Vector2(0, 0), new SizeF(2, 4), Matrix3x2.Identity);
            var transform = Matrix3x2.CreateTranslation(10, 20);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            CollectionAssert.Equal(
                new List<Vector2>
                    {
                        new(8, 16),
                        new(8, 24),
                        new(12, 24),
                        new(12, 16)
                    },
                result.Points);
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
            var rectangle = new OrientedRectangle(new Vector2(1, 2), new SizeF(2, 4), Matrix3x2.Identity);
            var transform =
                Matrix3x2.CreateRotationZ(MathHelper.PiOver2)
                *
                Matrix3x2.CreateTranslation(10, 20);

            var result = OrientedRectangle.Transform(rectangle, ref transform);

            Assert.Equal(8, result.Center.X, 6);
            Assert.Equal(21, result.Center.Y, 6);
            Assert.Equal(2, result.Radii.X, 6);
            Assert.Equal(4, result.Radii.Y, 6);
            Assert.Equal(Matrix3x2.CreateRotationZ(MathHelper.PiOver2), result.Orientation);
            CollectionAssert.Equal(
                new List<Vector2>
                    {
                        new(4, 23),
                        new(4, 19),
                        new(12, 19),
                        new(12, 23)
                    },
                result.Points);
        }
    }
}
