using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Primitives;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class Size2Tests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(0, 0).SetName(
                        "The empty size has the expected dimensions.");
                yield return
                    new TestCaseData(float.MinValue, float.MaxValue).SetName
                    (
                        "A non-empty size has the expected dimensions.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(float width, float height)
        {
            var size = new Size2(width, height);
            Assert.AreEqual(width, size.Width);
            Assert.AreEqual(height, size.Height);
        }

        public IEnumerable<TestCaseData> DimensionsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), 0, 0).SetName(
                        "The empty size has the expected dimensions.");
                yield return
                    new TestCaseData(new Size2(float.MinValue, float.MaxValue), float.MinValue, float.MaxValue).SetName
                    (
                        "A non-empty size has the expected dimensions.");
            }
        }

        [Test]
        [TestCaseSource(nameof(DimensionsTestCases))]
        public void Dimensions(Size2 size, float expectedWidth, float expecetedHeight)
        {
            Assert.AreEqual(expectedWidth, size.Width);
            Assert.AreEqual(expecetedHeight, size.Height);

            size.Width = 10;
            Assert.AreEqual(10, size.Width);

            size.Height = -10.123f;
            Assert.AreEqual(-10.123f, size.Height);
        }

        public IEnumerable<TestCaseData> AdditionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Size2(), new Size2()).SetName(
                        "The addition of two empty sizes is the empty size.");
                yield return
                    new TestCaseData(new Size2(5, 5), new Size2(15, 15), new Size2(20, 20)).SetName(
                        "The addition of two non-empty sizes is the expected size.");
            }
        }

        [Test]
        [TestCaseSource(nameof(AdditionTestCases))]
        public void Addition(Size2 size1, Size2 size2, Size2 expectedSize)
        {
            Assert.AreEqual(expectedSize, size1 + size2);
            Assert.AreEqual(expectedSize, Size2.Add(size1, size2));
        }

        public IEnumerable<TestCaseData> SubtractionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Size2(), new Size2()).SetName(
                        "The subtraction of two empty sizes is the empty size.");
                yield return
                    new TestCaseData(new Size2(5, 5), new Size2(15, 15), new Size2(-10, -10)).SetName(
                        "The subtraction of two non-empty sizes is the expected size.");
            }
        }

        [Test]
        [TestCaseSource(nameof(SubtractionTestCases))]
        public void Subtraction(Size2 size1, Size2 size2, Size2 expectedSize)
        {
            Assert.AreEqual(expectedSize, size1 - size2);
            Assert.AreEqual(expectedSize, Size2.Subtract(size1, size2));
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Size2(), true).SetName("Two empty sizes are equal.");
                yield return
                    new TestCaseData(new Size2(float.MinValue, float.MaxValue),
                        new Size2(float.MaxValue, float.MinValue), false).SetName(
                        "Two different non-empty sizes are not equal.");
                yield return
                    new TestCaseData(
                            new Size2(float.MinValue, float.MaxValue), new Size2(float.MinValue, float.MaxValue), true)
                        .SetName(
                            "Two identical non-empty sizes are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(Size2 size1, Size2 size2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(size1, size2) == expectedToBeEqual);
            Assert.IsTrue(size1 == size2 == expectedToBeEqual);
            Assert.IsFalse(size1 == size2 != expectedToBeEqual);
            Assert.IsTrue(size1.Equals(size2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), null, false).SetName("A size is not equal to a null object.");
                yield return
                    new TestCaseData(new Size2(), new object(), false).SetName(
                        "A size is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(Size2 size, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(size.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Size2(), true).SetName(
                        "Two empty sizes have the same hash code.");
                yield return
                    new TestCaseData(new Size2(50, 50), new Size2(50, 50), true).SetName(
                        "Two indentical non-empty sizes have the same hash code.");
                yield return
                    new TestCaseData(new Size2(0, 0), new Size2(50, 50), false).SetName(
                        "Two different non-empty sizes do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(Size2 size1, Size2 size2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = size1.GetHashCode();
            var hashCode2 = size2.GetHashCode();
            if (expectedThatHashCodesAreEqual)
                Assert.AreEqual(hashCode1, hashCode2);
            else
                Assert.AreNotEqual(hashCode1, hashCode2);
        }

        public IEnumerable<TestCaseData> ToPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Point2()).SetName("The empty size converted to a point is the zero point.");
                yield return
                    new TestCaseData(new Size2(float.MinValue, float.MaxValue), new Point2(float.MinValue, float.MaxValue)).SetName(
                        "A non-empty size converted to a point is the expected point.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ToPointTestCases))]
        public void ToPoint(Size2 size, Point2 expectedPoint)
        {
            var actualPoint = (Point2)size;
            Assert.AreEqual(expectedPoint, actualPoint);
        }

        public IEnumerable<TestCaseData> FromPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Size2()).SetName("The zero point converted to a size is the empty size.");
                yield return
                    new TestCaseData(new Point2(float.MinValue, float.MaxValue), new Size2(float.MinValue, float.MaxValue)).SetName(
                        "A non-zero point converted to a size is the expected size.");
            }
        }

        [Test]
        [TestCaseSource(nameof(FromPointTestCases))]
        public void FromPoint(Point2 point, Size2 expectedSize)
        {
            var actualSize = (Size2)point;
            Assert.AreEqual(expectedSize, actualSize);
        }

        public IEnumerable<TestCaseData> ToVectorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(), new Vector2()).SetName("The empty size converted to a vector is the zero vector.");
                yield return
                    new TestCaseData(new Size2(float.MinValue, float.MaxValue), new Vector2(float.MinValue, float.MaxValue)).SetName(
                        "A non-empty size converted to a vector is the expected vector.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ToVectorTestCases))]
        public void ToVector(Size2 size, Vector2 expectedVector)
        {
            var actualVector = (Vector2)size;
            Assert.AreEqual(expectedVector, actualVector);
        }

        public IEnumerable<TestCaseData> FromVectorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Vector2(), new Size2()).SetName("The zero vector converted to a size is the empty size.");
                yield return
                    new TestCaseData(new Vector2(float.MinValue, float.MaxValue), new Size2(float.MinValue, float.MaxValue)).SetName(
                        "A non-zero vector converted to a size is the expected size.");
            }
        }

        [Test]
        [TestCaseSource(nameof(FromVectorTestCases))]
        public void FromVector(Vector2 vector, Size2 expectedSize)
        {
            var actualSize = (Size2)vector;
            Assert.AreEqual(expectedSize, actualSize);
        }

        public IEnumerable<TestCaseData> StringCases
        {
            get
            {
                yield return
                    new TestCaseData(new Size2(),
                        string.Format(CultureInfo.CurrentCulture, "Width: {0}, Height: {1}", 0, 0)).SetName(
                        "The empty size has the expected string representation using the current culture.");
                yield return new TestCaseData(new Size2(5.1f, -5.123f),
                    string.Format(CultureInfo.CurrentCulture, "Width: {0}, Height: {1}", 5.1f, -5.123f)).SetName(
                    "A non-empty size has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(Size2 size, string expectedString)
        {
            var actualString = size.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
