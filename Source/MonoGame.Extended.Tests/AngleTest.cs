using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class AngleTest
    {
        private const double DELTA = 0.000000000001;
        [Test]
        public void ConstructorTest()
        {
            var value = 0.5;
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
            var radians = new Angle(Math.PI);
            Assert.AreEqual(180d, radians.Degrees, DELTA);
            Assert.AreEqual(200d, radians.Gradians, DELTA);
            Assert.AreEqual(0.5, radians.Revolutions, DELTA);
            //to radians
            var degrees = new Angle(180d, AngleType.Degree);
            var gradians = new Angle(200d, AngleType.Gradian);
            var revolutions = new Angle(0.5, AngleType.Revolution);
            Assert.AreEqual(Math.PI, degrees.Radians, DELTA);
            Assert.AreEqual(Math.PI, gradians.Radians, DELTA);
            Assert.AreEqual(Math.PI, revolutions.Radians, DELTA);
        }

        [Test]
        public void WrapTest()
        {
            for (var d = -10d; d < 10d; d += 0.1d)
            {
                var wrappositive = new Angle(d);
                wrappositive.WrapPositive();

                var wrap = new Angle(d);
                wrap.Wrap();

                Assert.GreaterOrEqual(wrappositive.Radians, 0);
                Assert.Less(wrappositive.Radians, 2d * Math.PI);

                Assert.GreaterOrEqual(wrap.Radians, -Math.PI);
                Assert.Less(wrap.Radians, Math.PI);
            }
        }

        [Test]
        public void VectorTest()
        {
            var angle = Angle.FromVector(Vector2.One);
            Assert.AreEqual(-Math.PI / 4d, angle.Radians, DELTA);
            Assert.AreEqual(10f, angle.ToVector(10f).Length());

            angle = Angle.FromVector(Vector2.UnitX);
            Assert.AreEqual(0, angle.Radians, DELTA);
            Assert.IsTrue(Vector2.UnitX.EqualsWithTolerance(angle.ToUnitVector()));

            angle = Angle.FromVector(-Vector2.UnitY);
            Assert.AreEqual(Math.PI / 2d, angle.Radians, DELTA);
            Assert.IsTrue((-Vector2.UnitY).EqualsWithTolerance(angle.ToUnitVector()));
        }

        [Test]
        public void EqualsTest()
        {
            var angle1 = new Angle(0);
            var angle2 = new Angle(Math.PI * 2d);
            Assert.IsTrue(angle1 == angle2);
            angle2.Radians = Math.PI * 4d;
            Assert.IsTrue(angle1.Equals(angle2));
        }

    }
}