using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests
{
    public class AngleTest
    {
        private const float _delta = 0.00001f;
        private readonly WithinDeltaEqualityComparer _withinDeltaEqualityComparer = new(_delta);

        [Fact]
        public void ConstructorTest()
        {
            const float value = 0.5f;

            // ReSharper disable once RedundantArgumentDefaultValue
            var radians = new Angle(value, AngleType.Radian);
            var degrees = new Angle(value, AngleType.Degree);
            var gradians = new Angle(value, AngleType.Gradian);
            var revolutions = new Angle(value, AngleType.Revolution);

            Assert.Equal(0.5f, radians.Radians, _withinDeltaEqualityComparer);
            Assert.Equal(0.5f, degrees.Degrees, _withinDeltaEqualityComparer);
            Assert.Equal(0.5f, gradians.Gradians, _withinDeltaEqualityComparer);
            Assert.Equal(0.5f, revolutions.Revolutions, _withinDeltaEqualityComparer);
        }

        [Fact]
        public void ConversionTest()
        {
            //from radians
            var radians = new Angle(MathHelper.Pi);
            Assert.Equal(180f, radians.Degrees, _withinDeltaEqualityComparer);
            Assert.Equal(200f, radians.Gradians, _withinDeltaEqualityComparer);
            Assert.Equal(0.5f, radians.Revolutions, _withinDeltaEqualityComparer);

            //to radians
            var degrees = new Angle(180f, AngleType.Degree);
            var gradians = new Angle(200f, AngleType.Gradian);
            var revolutions = new Angle(0.5f, AngleType.Revolution);

            Assert.Equal(MathHelper.Pi, degrees.Radians, _withinDeltaEqualityComparer);
            Assert.Equal(MathHelper.Pi, gradians.Radians, _withinDeltaEqualityComparer);
            Assert.Equal(MathHelper.Pi, revolutions.Radians, _withinDeltaEqualityComparer);
        }

        [Fact]
        public void WrapTest()
        {
            for (var f = -10f; f < 10f; f += 0.1f)
            {
                var wrappositive = new Angle(f);
                wrappositive.WrapPositive();

                var wrap = new Angle(f);
                wrap.Wrap();

                Assert.True(wrappositive.Radians >= 0);
                Assert.True(wrappositive.Radians < 2d * MathHelper.Pi);

                Assert.True(wrap.Radians >= -MathHelper.Pi);
                Assert.True(wrap.Radians < MathHelper.Pi);
            }
        }

        [Fact]
        public void VectorTest()
        {
            var angle = Angle.FromVector(Vector2.One);
            Assert.Equal(-MathHelper.Pi / 4f, angle.Radians, _withinDeltaEqualityComparer);
            Assert.Equal(10f, angle.ToVector(10f).Length());

            angle = Angle.FromVector(Vector2.UnitX);
            Assert.Equal(0, angle.Radians, _withinDeltaEqualityComparer);
            Assert.True(Vector2.UnitX.EqualsWithTolerence(angle.ToUnitVector()));

            angle = Angle.FromVector(-Vector2.UnitY);
            Assert.Equal(MathHelper.Pi / 2f, angle.Radians, _withinDeltaEqualityComparer);
            Assert.True((-Vector2.UnitY).EqualsWithTolerence(angle.ToUnitVector()));
        }

        [Fact]
        public void EqualsTest()
        {
            var angle1 = new Angle(0);
            var angle2 = new Angle(MathHelper.Pi * 2f);
            Assert.True(angle1 == angle2);
            angle2.Radians = MathHelper.Pi * 4f;
            Assert.True(angle1.Equals(angle2));
        }
    }
}
