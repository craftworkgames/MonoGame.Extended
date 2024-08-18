using Microsoft.Xna.Framework;
using Xunit;

namespace MonoGame.Extended.Tests
{
    public class Vector2ExtensionsTests
    {
        [Fact]
        public void Vector2_EqualsWithTolerence_Test()
        {
            var a = new Vector2(1f, 1f);
            var b = new Vector2(1.0000001f, 1.0000001f);

            Assert.False(a.Equals(b));
            Assert.True(a.EqualsWithTolerence(b));
        }

        [Fact]
        public void Vector2_NormalizedCopy_Test()
        {
            var a = new Vector2(5, -10);
            var b = a.NormalizedCopy();

            Assert.True(new Vector2(0.4472136f, -0.8944272f).EqualsWithTolerence(b));
        }

        [Fact]
        public void Vector2_Perpendicular_Test()
        {
            // http://mathworld.wolfram.com/PerpendicularVector.html
            var a = new Vector2(5, -10);
            var b = a.PerpendicularClockwise();
            var c = a.PerpendicularCounterClockwise();

            Assert.Equal(new Vector2(-10, -5), b);
            Assert.Equal(new Vector2(10, 5), c);
        }

#if FNA || KNI
        [Fact]
        public void Vector2_Rotate_90_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(90));

            Assert.True(new Vector2(10, 0).EqualsWithTolerence(b));
        }

        [Fact]
        public void Vector2_Rotate_360_Degrees_Test()
        {
            var a = new Vector2(0, 10);
            var b = a.Rotate(MathHelper.ToRadians(360));

            Assert.True(new Vector2(0, 10).EqualsWithTolerence(b));
        }

        [Fact]
        public void Vector2_Rotate_45_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(45));

            Assert.True(new Vector2(7.071068f, -7.071068f).EqualsWithTolerence(b));
        }

#endif
        [Fact]
        public void Vector2_Truncate_Test()
        {
            var a = new Vector2(10, 10);
            var b = a.Truncate(5);

            Assert.Equal(5f, b.Length(), 3);
        }

        [Fact]
        public void Vector2_IsNaN_Test()
        {
            var a = new Vector2(float.NaN, 10);
            var b = new Vector2(10, float.NaN);
            var c = new Vector2(float.NaN, float.NaN);
            var d = new Vector2(10, 10);

            Assert.True(a.IsNaN());
            Assert.True(b.IsNaN());
            Assert.True(c.IsNaN());
            Assert.False(d.IsNaN());
        }

        [Fact]
        public void Vector2_ToAngle_Test()
        {
            var a = new Vector2(0, -10);
            var b = new Vector2(10, 0);
#if FNA || KNI
            var c = -Vector2.UnitY.Rotate(MathHelper.ToRadians(45));
#else
            var c = -Vector2.UnitY;
            c.Rotate(MathHelper.ToRadians(45));
#endif

            Assert.Equal(MathHelper.ToRadians(0), a.ToAngle());
            Assert.Equal(MathHelper.ToRadians(90), b.ToAngle());
            Assert.Equal(MathHelper.ToRadians(45), c.ToAngle());
        }
    }
}
