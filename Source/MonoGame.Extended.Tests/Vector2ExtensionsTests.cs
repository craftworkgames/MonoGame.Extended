using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class Vector2ExtensionsTests
    {
        [Test]
        public void Vector2_Rotate_90_Degrees_Test()
        {
            var vector2 = new Vector2(0, -10);
            var expected = new Vector2(10, 0);
            var result = vector2.Rotate(MathHelper.ToRadians(90));
            
            Assert.IsTrue(Equals(expected, result));
        }

        [Test]
        public void Vector2_Rotate_360_Degrees_Test()
        {
            var vector2 = new Vector2(0, 10);
            var expected = new Vector2(0, 10);
            var result = vector2.Rotate(MathHelper.ToRadians(360));

            Assert.IsTrue(Equals(expected, result));
        }

        [Test]
        public void Vector2_Rotate_45_Degrees_Test()
        {
            var vector2 = new Vector2(0, -10);
            var expected = new Vector2(7.071068f, -7.071068f);
            var result = vector2.Rotate(MathHelper.ToRadians(45));

            Assert.IsTrue(Equals(expected, result));
        }

        private static bool Equals(Vector2 a, Vector2 b, float tolerence = 0.00001f)
        {
            return Math.Abs(a.X - b.X) <= tolerence && Math.Abs(a.Y - b.Y) <= tolerence;
        }
    }
}