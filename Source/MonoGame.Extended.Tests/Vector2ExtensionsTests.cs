using System;
using System.Collections.Generic;
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
            Assert.IsTrue(a.EqualsWithTolerence(b));
        }

        [Test]
        public void Vector2_NormalizedCopy_Test()
        {
            var a = new Vector2(5, -10);
            var b = a.NormalizedCopy();

            Assert.IsTrue(new Vector2(0.4472136f, -0.8944272f).EqualsWithTolerence(b));
        }

        [Test]
        public void Vector2_Perpendicular_Test()
        {
            // http://mathworld.wolfram.com/PerpendicularVector.html
            var a = new Vector2(5, -10);
            var b = a.PerpendicularClockwise();
            var c = a.PerpendicularCounterClockwise();

            Assert.AreEqual(new Vector2(-10, -5), b);
            Assert.AreEqual(new Vector2(10, 5), c);
        }

        [Test]
        public void Vector2_Rotate_90_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(90));

            Assert.IsTrue(new Vector2(10, 0).EqualsWithTolerence(b));
        }

        [Test]
        public void Vector2_Rotate_360_Degrees_Test()
        {
            var a = new Vector2(0, 10);
            var b = a.Rotate(MathHelper.ToRadians(360));

            Assert.IsTrue(new Vector2(0, 10).EqualsWithTolerence(b));
        }

        [Test]
        public void Vector2_Rotate_45_Degrees_Test()
        {
            var a = new Vector2(0, -10);
            var b = a.Rotate(MathHelper.ToRadians(45));

            Assert.IsTrue(new Vector2(7.071068f, -7.071068f).EqualsWithTolerence(b));
        }

        [Test]
        public void Vector2_Truncate_Test()
        {
            var a = new Vector2(10, 10);
            var b = a.Truncate(5);
            
            Assert.AreEqual(5f, b.Length(), 0.001f);
        }

        [Test]
        public void Vector2_IsNaN_Test()
        {
            var a = new Vector2(float.NaN, 10);
            var b = new Vector2(10, float.NaN);
            var c = new Vector2(float.NaN, float.NaN);
            var d = new Vector2(10, 10);

            Assert.IsTrue(a.IsNaN());
            Assert.IsTrue(b.IsNaN());
            Assert.IsTrue(c.IsNaN());
            Assert.IsFalse(d.IsNaN());
        }

        [Test]
        public void Vector2_ToAngle_Test()
        {
            var a = new Vector2(0, -10);
            var b = new Vector2(10, 0);
            var c = -Vector2.UnitY.Rotate(MathHelper.ToRadians(45));

            Assert.AreEqual(MathHelper.ToRadians(0), a.ToAngle());
            Assert.AreEqual(MathHelper.ToRadians(90), b.ToAngle());
            Assert.AreEqual(MathHelper.ToRadians(45), c.ToAngle());
        }
    }
}