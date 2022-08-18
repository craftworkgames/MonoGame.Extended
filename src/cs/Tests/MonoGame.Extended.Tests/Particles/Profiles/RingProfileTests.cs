using System;
using MonoGame.Extended.Particles.Profiles;
using Xunit;

namespace MonoGame.Extended.Tests.Particles.Profiles
{
    public class RingProfileTests
    {
        [Fact]
        public void ReturnsOffsetEqualToRadius()
        {
            var subject = new RingProfile
            {
                Radius = 10f
            };
            subject.GetOffsetAndHeading(out var offset, out _);

            var length = Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y);
            Assert.Equal(10f, length, 6);
        }

        [Fact]
        public void WhenRadiateIsTrue_HeadingIsEqualToNormalizedOffset()
        {
            var subject = new RingProfile
            {
                Radius = 10f,
                Radiate = Profile.CircleRadiation.Out
            };
            subject.GetOffsetAndHeading(out var offset, out var heading);

            Assert.Equal(heading.X, offset.X / 10, 6);
            Assert.Equal(heading.Y, offset.Y / 10, 6);

        }

    }
}
