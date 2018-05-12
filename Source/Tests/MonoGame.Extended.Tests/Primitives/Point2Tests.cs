//using System.Collections.Generic;
//using System.Globalization;
//using Microsoft.Xna.Framework;
//using Xunit;

//namespace MonoGame.Extended.Tests.Primitives
//{
//    
//    public class Point2Tests
//    {
//        public IEnumerable<TestCaseData> ConstructorTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(0, 0).SetName(
//                        "The zero point has the expected coordinates.");
//                yield return
//                    new TestCaseData(float.MinValue, float.MaxValue).SetName
//                    (
//                        "A non-zero point has the expected coordinates.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(ConstructorTestCases))]
//        public void Constructor(float x, float y)
//        {
//            var point = new Point2(x, y);
//            Assert.Equal(x, point.X);
//            Assert.Equal(y, point.Y);
//        }

//        public IEnumerable<TestCaseData> CoordinatesTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), 0, 0).SetName(
//                        "The zero point has the expected coordinates.");
//                yield return
//                    new TestCaseData(new Point2(float.MinValue, float.MaxValue), float.MinValue, float.MaxValue).SetName
//                    (
//                        "A non-zero point has the expected coordinates.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(CoordinatesTestCases))]
//        public void Coordinates(Point2 point, float expectedX, float expecetedY)
//        {
//            Assert.Equal(expectedX, point.X);
//            Assert.Equal(expecetedY, point.Y);

//            point.X = 10;
//            Assert.Equal(10, point.X);

//            point.Y = -10.123f;
//            Assert.Equal(-10.123f, point.Y);
//        }

//        public IEnumerable<TestCaseData> VectorAdditionTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Vector2(), new Point2()).SetName(
//                        "The addition of the zero point and the zero vector is the zero point.");
//                yield return
//                    new TestCaseData(new Point2(5, 5), new Vector2(15, 15), new Point2(20, 20)).SetName(
//                        "The addition of a non-zero point and a non-zero vector is the expected point.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(VectorAdditionTestCases))]
//        public void VectorAddition(Point2 point, Vector2 vector, Point2 expectedPoint)
//        {
//            Assert.Equal(expectedPoint, point + vector);
//            Assert.Equal(expectedPoint, Point2.Add(point, vector));
//        }

//        public IEnumerable<TestCaseData> VectorSubtractionTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Vector2(), new Point2()).SetName(
//                        "The vector subtraction of two zero points is the zero vector.");
//                yield return
//                    new TestCaseData(new Point2(5, 5), new Vector2(15, 15), new Point2(-10, -10)).SetName(
//                        "The vector subtraction of two non-zero points is the expected vector.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(VectorSubtractionTestCases))]
//        public void VectorSubtraction(Point2 point, Vector2 vector, Point2 expectedPoint)
//        {
//            Assert.Equal(expectedPoint, point - vector);
//            Assert.Equal(expectedPoint, Point2.Subtract(point, vector));
//        }


//        public IEnumerable<TestCaseData> DisplacementTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Point2(), new Vector2()).SetName(
//                        "The displacement between two zero points is the zero vector.");
//                yield return
//                    new TestCaseData(new Point2(5, 5), new Point2(15, 15), new Vector2(10, 10)).SetName(
//                        "The displacement between two non-zero points is the expected vector.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(DisplacementTestCases))]
//        public void Displacement(Point2 point1, Point2 point2, Vector2 expectedVector)
//        {
//            Assert.Equal(expectedVector, point2 - point1);
//            Assert.Equal(expectedVector, Point2.Displacement(point2, point1));
//        }

//        public IEnumerable<TestCaseData> SizeAdditionTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Size2(), new Point2()).SetName(
//                        "The size addition of the zero point with the empty size is the zero point.");
//                yield return
//                    new TestCaseData(new Point2(5, 5), new Size2(15, 15), new Point2(20, 20)).SetName(
//                        "The size addition of a non-zero point with a non-empty size is the expected point.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(SizeAdditionTestCases))]
//        public void SizeAdditon(Point2 point, Size2 size, Point2 expectedPoint)
//        {
//            Assert.Equal(expectedPoint, point + size);
//            Assert.Equal(expectedPoint, Point2.Add(point, size));
//        }

//        public IEnumerable<TestCaseData> SizeSubtractionTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Size2(), new Point2()).SetName(
//                        "The size substraction of the zero point with the empty size is the zero point.");
//                yield return
//                    new TestCaseData(new Point2(5, 5), new Size2(15, 15), new Point2(-10, -10)).SetName(
//                        "The size subscration of a non-zero point with a non-empty size is the expected point.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(SizeSubtractionTestCases))]
//        public void SizeSubtraction(Point2 point, Size2 size, Point2 expectedPoint)
//        {
//            Assert.Equal(expectedPoint, point - size);
//            Assert.Equal(expectedPoint, Point2.Subtract(point, size));
//        }

//        public IEnumerable<TestCaseData> MinimumTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Point2(), new Point2()).SetName(
//                        "The minimum coordinates of two zero points is the coordinates of the zero point.");
//                yield return
//                    new TestCaseData(new Point2(float.MaxValue, float.MinValue), new Point2(int.MaxValue, int.MinValue),
//                        new Point2(int.MaxValue, float.MinValue)).SetName(
//                        "The minimum coordaintes of two non-zero points is the expected coordinates.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(MinimumTestCases))]
//        public void Minimum(Point2 point1, Point2 point2, Point2 expectedPoint)
//        {
//            var actualPoint = Point2.Minimum(point1, point2);
//            Assert.Equal(expectedPoint, actualPoint);
//        }

//        public IEnumerable<TestCaseData> MaximumTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Point2(), new Point2()).SetName(
//                        "The maximum coordinates of two zero points is the coordinates of the zero point.");
//                yield return
//                    new TestCaseData(new Point2(float.MaxValue, float.MinValue), new Point2(int.MaxValue, int.MinValue),
//                        new Point2(float.MaxValue, int.MinValue)).SetName(
//                        "The maximum coordaintes of two non-zero points is the expected coordinates.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(MaximumTestCases))]
//        public void Maximum(Point2 point1, Point2 point2, Point2 expectedPoint)
//        {
//            var actualPoint = Point2.Maximum(point1, point2);
//            Assert.Equal(expectedPoint, actualPoint);
//        }

//        public IEnumerable<TestCaseData> EqualityTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Point2(), true).SetName("Two zero points are equal.");
//                yield return
//                    new TestCaseData(new Point2(float.MinValue, float.MaxValue),
//                        new Point2(float.MaxValue, float.MinValue), false).SetName(
//                        "Two different non-zero points are not equal.");
//                yield return
//                    new TestCaseData(
//                            new Point2(float.MinValue, float.MaxValue), new Point2(float.MinValue, float.MaxValue), true)
//                        .SetName(
//                            "Two identical non-zero points are equal.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(EqualityTestCases))]
//        public void Equality(Point2 point1, Point2 point2, bool expectedToBeEqual)
//        {
//            Assert.IsTrue(Equals(point1, point2) == expectedToBeEqual);
//            Assert.IsTrue(point1 == point2 == expectedToBeEqual);
//            Assert.IsFalse(point1 == point2 != expectedToBeEqual);
//            Assert.IsTrue(point1.Equals(point2) == expectedToBeEqual);

//            if (expectedToBeEqual)
//                Assert.Equal(point1.GetHashCode(), point2.GetHashCode());
//        }

//        public IEnumerable<TestCaseData> InequalityTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), null, false).SetName("A point is not equal to a null object.");
//                yield return
//                    new TestCaseData(new Point2(), new object(), false).SetName(
//                        "A point is not equal to an instantiated object.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(InequalityTestCases))]
//        public void Inequality(Point2 point, object obj, bool expectedToBeEqual)
//        {
//            Assert.IsTrue(point.Equals(obj) == expectedToBeEqual);
//        }

//        public IEnumerable<TestCaseData> HashCodeTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Point2(), true).SetName(
//                        "Two zero points have the same hash code.");
//                yield return
//                    new TestCaseData(new Point2(50, 50), new Point2(50, 50), true).SetName(
//                        "Two indentical non-zero points have the same hash code.");
//                yield return
//                    new TestCaseData(new Point2(0, 0), new Point2(50, 50), false).SetName(
//                        "Two different non-zero points do not have the same hash code.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(HashCodeTestCases))]
//        public void HashCode(Point2 point1, Point2 point2, bool expectedThatHashCodesAreEqual)
//        {
//            var hashCode1 = point1.GetHashCode();
//            var hashCode2 = point2.GetHashCode();
//            if (expectedThatHashCodesAreEqual)
//                Assert.Equal(hashCode1, hashCode2);
//            else
//                Assert.AreNotEqual(hashCode1, hashCode2);
//        }

//        public IEnumerable<TestCaseData> ToVectorTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(), new Vector2()).SetName(
//                        "The zero point converted to a vector is the zero vector.");
//                yield return
//                    new TestCaseData(new Point2(float.MinValue, float.MaxValue),
//                        new Vector2(float.MinValue, float.MaxValue)).SetName(
//                        "A non-zero point converted to a vector is the expected vector.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(ToVectorTestCases))]
//        public void ToVector(Point2 point, Vector2 expectedVector)
//        {
//            var actualVector = (Vector2)point;
//            Assert.Equal(expectedVector, actualVector);
//        }

//        public IEnumerable<TestCaseData> FromVectorTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Vector2(), new Point2()).SetName(
//                        "The zero vector converted to a point is the zero point.");
//                yield return
//                    new TestCaseData(new Vector2(float.MinValue, float.MaxValue),
//                        new Point2(float.MinValue, float.MaxValue)).SetName(
//                        "A non-zero vector converted to a point is the expected point.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(FromVectorTestCases))]
//        public void FromVector(Vector2 vector, Point2 expectedPoint)
//        {
//            var actualPoint = (Point2)vector;
//            Assert.Equal(expectedPoint, actualPoint);
//        }

//        public IEnumerable<TestCaseData> StringCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Point2(),
//                        string.Format(CultureInfo.CurrentCulture, "({0}, {1})", 0, 0)).SetName(
//                        "The zero point has the expected string representation using the current culture.");
//                yield return new TestCaseData(new Point2(5.1f, -5.123f),
//                    string.Format(CultureInfo.CurrentCulture, "({0}, {1})", 5.1f, -5.123f)).SetName(
//                    "A non-zero point has the expected string representation using the current culture.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(StringCases))]
//        public void String(Point2 point, string expectedString)
//        {
//            var actualString = point.ToString();
//            Assert.Equal(expectedString, actualString);
//        }
//    }
//}
