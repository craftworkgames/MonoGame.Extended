using System;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void ConstructorTest()
        {
            //can pass min < max
            Assert.DoesNotThrow(() => new Range<int>(10, 100));
            //can't pass min > max
            Assert.Throws<ArgumentException>(() => new Range<int>(100, 10));
            //can pass min == max
            Assert.DoesNotThrow(() => new Range<int>(10, 10));
        }

        [Test]
        public void DegenerateTest()
        {
            var proper = new Range<double>(0, 1);
            Assert.IsTrue(proper.IsProper);
            Assert.IsFalse(proper.IsDegenerate);

            var degenerate = new Range<double>(1, 1);
            Assert.IsFalse(degenerate.IsProper);
            Assert.IsTrue(degenerate.IsDegenerate);
        }

        [Test]
        public void IntegerTest()
        {
            var range = new Range<int>(10, 100);

            Assert.AreEqual(range.Min, 10);
            Assert.AreEqual(range.Max, 100);

            for (var i = 10; i <= 100; i++)
            {
                Assert.IsTrue(range.IsInBetween(i));
            }

            Assert.IsFalse(range.IsInBetween(9));
            Assert.IsFalse(range.IsInBetween(101));
            Assert.IsFalse(range.IsInBetween(10, true));
            Assert.IsFalse(range.IsInBetween(100, maxValueExclusive: true));
        }

        [Test]
        public void FloatTest()
        {
            var range = new Range<float>(0f, 1f);

            Assert.AreEqual(range.Min, 0f);
            Assert.AreEqual(range.Max, 1f);

            for (float i = 0; i <= 1f; i += 0.001f)
            {
                Assert.IsTrue(range.IsInBetween(i));
            }

            Assert.IsFalse(range.IsInBetween(-float.Epsilon));
            Assert.IsFalse(range.IsInBetween(1.00001f));

            Assert.IsFalse(range.IsInBetween(0f, true));
            Assert.IsFalse(range.IsInBetween(1f, maxValueExclusive: true));
        }

        [Test]
        public void OperatorTest()
        {
            var rangeA = new Range<int>(0, 1);
            var rangeB = new Range<int>(0, 1);
            var rangeC = new Range<int>(1, 2);
            var rangeD = new Range<double>(0, 1);

            Assert.IsTrue(rangeA == rangeB);
            Assert.IsFalse(rangeA == rangeC);

            Assert.IsFalse(rangeA != rangeB);
            Assert.IsTrue(rangeA != rangeC);

            Assert.IsTrue(rangeA.Equals(rangeB));
            Assert.IsFalse(rangeA.Equals(rangeC));
            Assert.IsFalse(rangeA.Equals(rangeD));

            Range<int> implict = 1;
            Assert.AreEqual(implict.Max, 1);
            Assert.AreEqual(implict.Min, 1);
        }

        [Test]
        public void ToStringTest()
        {
            var range = new Range<float>(0, 1);

            Assert.AreEqual(range.ToString(), "Range<Single> [0 1]");
        }
    }
}