using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Profiles;

namespace MonoGame.Extended.Tests.Particles.Profiles
{
    public class RingProfileTests
    {
        [Fact]
        public unsafe void ReturnsOffsetEqualToRadius()
        {
            RingProfile subject = new RingProfile
            {
                Radius = 10f
            };

            Vector2 offset;
            Vector2 heading;
            subject.GetOffsetAndHeading(&offset, &heading);

            double length = Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y);
            Assert.Equal(10.0f, length, precision: 5);
        }

        [Fact]
        public unsafe void WhenRadiateIsTrue_HeadingIsEqualToNormalizedOffset()
        {
            RingProfile subject = new RingProfile
            {
                Radius = 10f,
                Radiate = CircleRadiation.Out
            };

            Vector2 offset;
            Vector2 heading;
            subject.GetOffsetAndHeading(&offset, &heading);

            Assert.Equal(heading.X, offset.X / 10, precision: 5);
            Assert.Equal(heading.Y, offset.Y / 10, precision: 5);
        }
    }
}
