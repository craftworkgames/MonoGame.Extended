using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Profiles;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Particles.Profiles
{
    [TestFixture]
    public class PointProfileTests
    {
        [Test]
        public void ReturnsZeroOffset()
        {
            var subject = new PointProfile();

            Vector2 offset, heading;
            subject.GetOffsetAndHeading(out offset, out heading);

            Assert.AreEqual(0f, offset.X);
            Assert.AreEqual(0f, offset.Y);
        }

        [Test]
        public void ReturnsHeadingAsUnitVector()
        {
            var subject = new PointProfile();

            Vector2 offset, heading;
            subject.GetOffsetAndHeading(out offset, out heading);

            var length = Math.Sqrt(heading.X * heading.X + heading.Y * heading.Y);
            Assert.AreEqual(1f, length, 0.000001);
        }

    }
}