using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Profiles;

namespace MonoGame.Extended.Tests.Particles.Profiles
{
    public class PointProfileTests
    {
        [Fact]
        public unsafe void ReturnsZeroOffset()
        {
            PointProfile subject = new PointProfile();

            Vector2 offset;
            Vector2 heading;
            subject.GetOffsetAndHeading(&offset, &heading);

            Assert.Equal(0f, offset.X);
            Assert.Equal(0f, offset.Y);
        }

        [Fact]
        public unsafe void ReturnsHeadingAsUnitVector()
        {
            PointProfile subject = new PointProfile();

            Vector2 offset;
            Vector2 heading;
            subject.GetOffsetAndHeading(&offset, &heading);

            double length = Math.Sqrt(heading.X * heading.X + heading.Y * heading.Y);
            Assert.Equal(1f, length, 6);
        }
    }
}
