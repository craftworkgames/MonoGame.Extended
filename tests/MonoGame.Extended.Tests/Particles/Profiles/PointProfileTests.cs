using System;
using MonoGame.Extended.Particles.Profiles;
using Xunit;

namespace MonoGame.Extended.Tests.Particles.Profiles
{

    public class PointProfileTests
    {
        [Fact]
        public void ReturnsZeroOffset()
        {
            var subject = new PointProfile();

            subject.GetOffsetAndHeading(out var offset, out _);

            Assert.Equal(0f, offset.X);
            Assert.Equal(0f, offset.Y);
        }

        [Fact]
        public void ReturnsHeadingAsUnitVector()
        {
            var subject = new PointProfile();

            subject.GetOffsetAndHeading(out _, out var heading);

            var length = Math.Sqrt(heading.X * heading.X + heading.Y * heading.Y);
            Assert.Equal(1f, length, 6);
        }

    }
}
