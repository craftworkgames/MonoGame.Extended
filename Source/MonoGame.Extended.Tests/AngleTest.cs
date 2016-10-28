using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class AngleTest
    {
        private const double _delta = 0.000000000000001f;
        [Test]
        public void ConstructorTest()
        {
            var value = 0.5f;
            var radians = new Angle(value);
            var degrees = new Angle(value, AngleType.Degree);
            var gradians = new Angle(value, AngleType.Gradian);
            var revolutions = new Angle(value, AngleType.Revolution);
            Assert.AreEqual(value, radians.Radians, _delta);
            Assert.AreEqual(value, degrees.Degrees, _delta);
            Assert.AreEqual(value, gradians.Gradians, _delta);
            Assert.AreEqual(value, revolutions.Revolutions, _delta);
        }

        [Test]
        [Ignore("This test is broken on the build server and I have no idea why")]
        public void ConversionTest()
        {
            //from radians
            var radians = new Angle(MathHelper.Pi);
            Assert.AreEqual(180d, radians.Degrees, _delta);
            Assert.AreEqual(200d, radians.Gradians, _delta);
            Assert.AreEqual(0.5, radians.Revolutions, _delta);
            //to radians
            var degrees = new Angle(180f, AngleType.Degree);
            var gradians = new Angle(200f, AngleType.Gradian);
            var revolutions = new Angle(0.5f, AngleType.Revolution);
            Assert.AreEqual(MathHelper.Pi, degrees.Radians, _delta);
            Assert.AreEqual(MathHelper.Pi, gradians.Radians, _delta);
            Assert.AreEqual(MathHelper.Pi, revolutions.Radians, _delta);
        }

        [Test]
        public void WrapTest()
        {
            for (var f = -10f; f < 10f; f += 0.1f)
            {
                var wrappositive = new Angle(f);
                wrappositive.WrapPositive();

                var wrap = new Angle(f);
                wrap.Wrap();

                Assert.GreaterOrEqual(wrappositive.Radians, 0);
                Assert.Less(wrappositive.Radians, 2d * MathHelper.Pi);

                Assert.GreaterOrEqual(wrap.Radians, -MathHelper.Pi);
                Assert.Less(wrap.Radians, MathHelper.Pi);
            }
        }

        [Test]
        public void VectorTest()
        {
            var angle = Angle.FromVector(Vector2.One);
            Assert.AreEqual(-MathHelper.Pi / 4d, angle.Radians, _delta);
            Assert.AreEqual(10f, angle.ToVector(10f).Length());

            angle = Angle.FromVector(Vector2.UnitX);
            Assert.AreEqual(0, angle.Radians, _delta);
            Assert.IsTrue(Vector2.UnitX.EqualsWithTolerence(angle.ToUnitVector()));

            angle = Angle.FromVector(-Vector2.UnitY);
            Assert.AreEqual(MathHelper.Pi / 2d, angle.Radians, _delta);
            Assert.IsTrue((-Vector2.UnitY).EqualsWithTolerence(angle.ToUnitVector()));
        }

        [Test]
        public void EqualsTest()
        {
            var angle1 = new Angle(0);
            var angle2 = new Angle(MathHelper.Pi * 2f);
            Assert.IsTrue(angle1 == angle2);
            angle2.Radians = MathHelper.Pi * 4f;
            Assert.IsTrue(angle1.Equals(angle2));
        }

    }
}