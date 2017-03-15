using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Profiles;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Particles.Profiles
{
    [TestFixture]
    public class RingProfileTests
    {
        [Test]
        public void ReturnsOffsetEqualToRadius()
        {
            var subject = new RingProfile
            {
                Radius = 10f
            };
            Vector2 offset, heading;
            subject.GetOffsetAndHeading(out offset, out heading);

            var length = Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y);
            Assert.AreEqual(10f, length, 0.000001);
        }

        [Test]
        public void WhenRadiateIsTrue_HeadingIsEqualToNormalizedOffset()
        {
            var subject = new RingProfile
            {
                Radius = 10f,
                Radiate = Profile.CircleRadiation.Out
            };
            Vector2 offset, heading;
            subject.GetOffsetAndHeading(out offset, out heading);

            Assert.AreEqual(heading.X, offset.X / 10, 0.000001);
            Assert.AreEqual(heading.Y, offset.Y / 10, 0.000001);

        }

    }
}