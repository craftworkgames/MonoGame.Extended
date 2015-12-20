using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class Vector2ExtensionsTests
    {
        [Test]
        public void Vector2_EqualsWithTolerence_Test()
        {
            var a = new Vector2(1f, 1f);
            var b = new Vector2(1.0000001f, 1.0000001f);

            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a.EqualsWithTolerance(b));
        }

        [Test]
        public void Vector2_NormalizedCopy_Test()
        {
            var a = new Vector2(5, -10);
            var b = a.NormalizedCopy();

            Assert.IsTrue(new Vector2(0.4472136f, -0.8944272f).EqualsWithTolerance(b));
        }

        [Test]
        public void Vector2_Perpendicular_Test()
        {
            var a = new Vector2(5, -10);
            var b = a.PerpendicularClockwise();
            var c = a.PerpendicularCounterClockwise();

            Assert.AreEqual(new Vector2(10, 5), b);
            Assert.AreEqual(new Vector2(-10, -5), c);
        }

        [Test]
        public void Vector2_Rotate_90_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(90));

            Assert.IsTrue(new Vector2(10, 0).EqualsWithTolerance(b));
        }

        [Test]
        public void Vector2_Rotate_360_Degrees_Test()
        {
            var a = new Vector2(0, 10);
            var b = a.Rotate(MathHelper.ToRadians(360));

            Assert.IsTrue(new Vector2(0, 10).EqualsWithTolerance(b));
        }

        [Test]
        public void Vector2_Rotate_45_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(45));

            Assert.IsTrue(new Vector2(7.071068f, -7.071068f).EqualsWithTolerance(b));
        }
    }
}