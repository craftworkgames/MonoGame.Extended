using System;
using Xunit;

namespace MonoGame.Extended.Tests
{

    public class RangeTests
    {
        [Fact]
        public void ConstructorTest()
        {
            //can pass min < max
            var unused = new Range<int>(10, 100);
            //can't pass min > max
            Assert.Throws<ArgumentException>(() => new Range<int>(100, 10));
            //can pass min == max
            var unused1 = new Range<int>(10, 10);
        }

        [Fact]
        public void DegenerateTest()
        {
            var proper = new Range<double>(0, 1);
            Assert.True(proper.IsProper);
            Assert.False(proper.IsDegenerate);

            var degenerate = new Range<double>(1, 1);
            Assert.False(degenerate.IsProper);
            Assert.True(degenerate.IsDegenerate);
        }

        [Fact]
        public void IntegerTest()
        {
            var range = new Range<int>(10, 100);

            Assert.Equal(10, range.Min);
            Assert.Equal(100, range.Max);

            for (var i = 10; i <= 100; i++)
            {
                Assert.True(range.IsInBetween(i));
            }

            Assert.False(range.IsInBetween(9));
            Assert.False(range.IsInBetween(101));
            Assert.False(range.IsInBetween(10, true));
            Assert.False(range.IsInBetween(100, maxValueExclusive: true));
        }

        [Fact]
        public void FloatTest()
        {
            var range = new Range<float>(0f, 1f);

            Assert.Equal(0f, range.Min);
            Assert.Equal(1f, range.Max);

            for (float i = 0; i <= 1f; i += 0.001f)
            {
                Assert.True(range.IsInBetween(i));
            }

            Assert.False(range.IsInBetween(-float.Epsilon));
            Assert.False(range.IsInBetween(1.00001f));

            Assert.False(range.IsInBetween(0f, true));
            Assert.False(range.IsInBetween(1f, maxValueExclusive: true));
        }

        [Fact]
        public void OperatorTest()
        {
            var rangeA = new Range<int>(0, 1);
            var rangeB = new Range<int>(0, 1);
            var rangeC = new Range<int>(1, 2);
            var rangeD = new Range<double>(0, 1);

            Assert.True(rangeA == rangeB);
            Assert.False(rangeA == rangeC);

            Assert.False(rangeA != rangeB);
            Assert.True(rangeA != rangeC);

            Assert.True(rangeA.Equals(rangeB));
            Assert.False(rangeA.Equals(rangeC));
            Assert.False(rangeA.Equals(rangeD));

            Range<int> implict = 1;
            Assert.Equal(1, implict.Max);
            Assert.Equal(1, implict.Min);
        }

        [Fact]
        public void ToStringTest()
        {
            var range = new Range<float>(0, 1);

            Assert.Equal("Range<Single> [0 1]", range.ToString());
        }
    }
}
