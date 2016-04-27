using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class RangeTest
    {
        [Test]
        public void ConstructorTest() {
            //can pass min < max
            Assert.DoesNotThrow(() => new Range<int>(10, 100));
            //can't pass min > max
            Assert.Throws<ArgumentException>(() => new Range<int>(100, 10));
            //can pass min == max
            Assert.DoesNotThrow(() => new Range<int>(10, 10));
        }

        [Test]
        public void IntegerTest() {
            var range = new Range<int>(10, 100);

            Assert.AreEqual(range.Min, 10);
            Assert.AreEqual(range.Max, 100);

            for (int i = 10; i <= 100; i++) {
                Assert.IsTrue(range.IsInBetween(i));
            }

            Assert.IsFalse(range.IsInBetween(9));
            Assert.IsFalse(range.IsInBetween(101));
            Assert.IsFalse(range.IsInBetween(10, true));
            Assert.IsFalse(range.IsInBetween(100, maxValueExclusive: true));
        }


        [Test]
        public void FloatTest() {
            var range = new Range<float>(0f, 1f);

            Assert.AreEqual(range.Min, 0f);
            Assert.AreEqual(range.Max, 1f);

            for (float i = 0; i <= 1f; i += 0.001f) {
                Assert.IsTrue(range.IsInBetween(i));
            }

            Assert.IsFalse(range.IsInBetween(-float.Epsilon));
            Assert.IsFalse(range.IsInBetween(1.00001f));

            Assert.IsFalse(range.IsInBetween(0f, true));
            Assert.IsFalse(range.IsInBetween(1f, maxValueExclusive: true));
        }

    }
}