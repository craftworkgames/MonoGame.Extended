using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class AngleTest
    {
        private const float DELTA = 0.0000001f;
        [Test]
        public void ConstructorTest()
        {
            var value = 0.5f;
            var radians = new Angle(value);
            var degrees = new Angle(value, AngleType.Degree);
            var gradians = new Angle(value, AngleType.Gradian);
            var revolutions = new Angle(value, AngleType.Revolution);
            Assert.AreEqual(value, radians.Radians, DELTA);
            Assert.AreEqual(value, degrees.Degrees, DELTA);
            Assert.AreEqual(value, gradians.Gradians, DELTA);
            Assert.AreEqual(value, revolutions.Revolutions, DELTA);
        }

        [Test]
        public void ConversionTest()
        {
            //from radians
            var radians = new Angle(MathHelper.Pi);
            Assert.AreEqual(180d, radians.Degrees, DELTA);
            Assert.AreEqual(200d, radians.Gradians, DELTA);
            Assert.AreEqual(0.5, radians.Revolutions, DELTA);
            //to radians
            var degrees = new Angle(180f, AngleType.Degree);
            var gradians = new Angle(200f, AngleType.Gradian);
            var revolutions = new Angle(0.5f, AngleType.Revolution);
            Assert.AreEqual(MathHelper.Pi, degrees.Radians, DELTA);
            Assert.AreEqual(MathHelper.Pi, gradians.Radians, DELTA);
            Assert.AreEqual(MathHelper.Pi, revolutions.Radians, DELTA);
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
            Assert.AreEqual(-MathHelper.Pi / 4d, angle.Radians, DELTA);
            Assert.AreEqual(10f, angle.ToVector(10f).Length());

            angle = Angle.FromVector(Vector2.UnitX);
            Assert.AreEqual(0, angle.Radians, DELTA);
            Assert.IsTrue(Vector2.UnitX.EqualsWithTolerance(angle.ToUnitVector()));

            angle = Angle.FromVector(-Vector2.UnitY);
            Assert.AreEqual(MathHelper.Pi / 2d, angle.Radians, DELTA);
            Assert.IsTrue((-Vector2.UnitY).EqualsWithTolerance(angle.ToUnitVector()));
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