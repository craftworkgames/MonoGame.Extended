//using Microsoft.Xna.Framework;
//using Xunit;

//namespace MonoGame.Extended.Tests
//{
//    
//    public class AngleTest
//    {
//        private const float _delta = 0.00001f;

//        [Fact]
//        public void ConstructorTest()
//        {
//            const float value = 0.5f;
//            // ReSharper disable once RedundantArgumentDefaultValue
//            var radians = new Angle(value, AngleType.Radian);
//            var degrees = new Angle(value, AngleType.Degree);
//            var gradians = new Angle(value, AngleType.Gradian);
//            var revolutions = new Angle(value, AngleType.Revolution);

//            Assert.That(0.5f, Is.EqualTo(radians.Radians).Within(_delta));
//            Assert.That(0.5f, Is.EqualTo(degrees.Degrees).Within(_delta));
//            Assert.That(0.5f, Is.EqualTo(gradians.Gradians).Within(_delta));
//            Assert.That(0.5f, Is.EqualTo(revolutions.Revolutions).Within(_delta));
//        }

//        [Fact]
//        public void ConversionTest()
//        {
//            //from radians
//            var radians = new Angle(MathHelper.Pi);
//            Assert.Equal(180f, radians.Degrees, _delta);
//            Assert.Equal(200f, radians.Gradians, _delta);
//            Assert.Equal(0.5f, radians.Revolutions, _delta);
//            //to radians
//            var degrees = new Angle(180f, AngleType.Degree);
//            var gradians = new Angle(200f, AngleType.Gradian);
//            var revolutions = new Angle(0.5f, AngleType.Revolution);

//            Assert.That(MathHelper.Pi, Is.EqualTo(degrees.Radians).Within(_delta));
//            Assert.That(MathHelper.Pi, Is.EqualTo(gradians.Radians).Within(_delta));
//            Assert.That(MathHelper.Pi, Is.EqualTo(revolutions.Radians).Within(_delta));
//        }

//        [Fact]
//        public void WrapTest()
//        {
//            for (var f = -10f; f < 10f; f += 0.1f)
//            {
//                var wrappositive = new Angle(f);
//                wrappositive.WrapPositive();

//                var wrap = new Angle(f);
//                wrap.Wrap();

//                Assert.GreaterOrEqual(wrappositive.Radians, 0);
//                Assert.Less(wrappositive.Radians, 2d * MathHelper.Pi);

//                Assert.GreaterOrEqual(wrap.Radians, -MathHelper.Pi);
//                Assert.Less(wrap.Radians, MathHelper.Pi);
//            }
//        }

//        [Fact]
//        public void VectorTest()
//        {
//            var angle = Angle.FromVector(Vector2.One);
//            Assert.Equal(-MathHelper.Pi / 4d, angle.Radians, _delta);
//            Assert.Equal(10f, angle.ToVector(10f).Length());

//            angle = Angle.FromVector(Vector2.UnitX);
//            Assert.Equal(0, angle.Radians, _delta);
//            Assert.IsTrue(Vector2.UnitX.EqualsWithTolerence(angle.ToUnitVector()));

//            angle = Angle.FromVector(-Vector2.UnitY);
//            Assert.Equal(MathHelper.Pi / 2d, angle.Radians, _delta);
//            Assert.IsTrue((-Vector2.UnitY).EqualsWithTolerence(angle.ToUnitVector()));
//        }

//        [Fact]
//        public void EqualsTest()
//        {
//            var angle1 = new Angle(0);
//            var angle2 = new Angle(MathHelper.Pi * 2f);
//            Assert.IsTrue(angle1 == angle2);
//            angle2.Radians = MathHelper.Pi * 4f;
//            Assert.IsTrue(angle1.Equals(angle2));
//        }

//    }
//}