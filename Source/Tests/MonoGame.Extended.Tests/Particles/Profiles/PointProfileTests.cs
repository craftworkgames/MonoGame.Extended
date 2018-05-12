//using System;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Particles.Profiles;
//using Xunit;

//namespace MonoGame.Extended.Tests.Particles.Profiles
//{
//    
//    public class PointProfileTests
//    {
//        [Fact]
//        public void ReturnsZeroOffset()
//        {
//            var subject = new PointProfile();

//            Vector2 offset, heading;
//            subject.GetOffsetAndHeading(out offset, out heading);

//            Assert.Equal(0f, offset.X);
//            Assert.Equal(0f, offset.Y);
//        }

//        [Fact]
//        public void ReturnsHeadingAsUnitVector()
//        {
//            var subject = new PointProfile();

//            Vector2 offset, heading;
//            subject.GetOffsetAndHeading(out offset, out heading);

//            var length = Math.Sqrt(heading.X * heading.X + heading.Y * heading.Y);
//            Assert.Equal(1f, length, 0.000001);
//        }

//    }
//}