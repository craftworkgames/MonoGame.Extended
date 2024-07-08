//using System.Collections.Generic;
//using System.Globalization;
//using Xunit;

//namespace MonoGame.Extended.Tests.Primitives
//{
//    public class Segment2DTests
//    {
//        public IEnumerable<TestCaseData> ConstructorTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Vector2(), new Vector2()).SetName(
//                        "The empty segment has expected starting and ending points.");
//                yield return
//                    new TestCaseData(
//                        new Vector2(float.MaxValue, float.MinValue),
//                        new Vector2(int.MaxValue, int.MinValue)).SetName(
//                        "A non-empty segment has the expected starting and ending points.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(ConstructorTestCases))]
//        public void Constructor(Vector2 startingPoint, Vector2 endingPoint)
//        {
//            var segment = new Segment2(startingPoint, endingPoint);
//            Assert.Equal(startingPoint, segment.Start);
//            Assert.Equal(endingPoint, segment.End);
//        }

//        public IEnumerable<TestCaseData> ClosestPointTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), new Vector2(), new Vector2()).SetName(
//                        "The closest point on the empty segment to the zero point is the zero point.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(200, 0)), new Vector2(-100, 200),
//                        new Vector2(0, 0)).SetName(
//                        "The closest point on a non-empty segment to a point which is projected beyond the start of the segment is the segment's starting point.")
//                    ;
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(200, 0)), new Vector2(400, 200),
//                        new Vector2(200, 0)).SetName(
//                        "The closest point on a non-empty segment to a point which is projected beyond the end of the segment is the segment's ending point.")
//                    ;
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(200, 0)), new Vector2(100, 200),
//                        new Vector2(100, 0)).SetName(
//                        "The closest point on a non-empty segment to a point which is projected inside the segment is the projected point.")
//                    ;
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(ClosestPointTestCases))]
//        public void ClosestPoint(Segment2 segment, Vector2 point, Vector2 expectedClosestPoint)
//        {
//            var actualClosestPoint = segment.ClosestPointTo(point);
//            Assert.Equal(expectedClosestPoint, actualClosestPoint);
//        }

//        public IEnumerable<TestCaseData> SquaredDistanceToPointTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), new Vector2(), 0).SetName(
//                        "The squared distance of the zero point to the empty segment is 0.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(20, 0)), new Vector2(-10, 20), 500)
//                        .SetName(
//                            "The squared distance of a point projected beyond the start of a non-empty segment is the squared distance from the segment's starting point to the point.")
//                    ;
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(20, 0)), new Vector2(40, 20), 400)
//                        .SetName(
//                            "The squared distance of a point projected beyond the end of a non-empty segment is the squared distance from the segment's ending point to the point.")
//                    ;
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(20, 0)), new Vector2(10, 25), 625).SetName
//                    (
//                        "The squared distance of a point projected inside a non-empty segment is the squared distance from the projected point to the point.")
//                    ;
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(SquaredDistanceToPointTestCases))]
//        public void SquaredDistanceToPoint(Segment2 segment, Vector2 point,
//            float expectedDistance)
//        {
//            var actualDistance = segment.SquaredDistanceTo(point);
//            Assert.Equal(expectedDistance, actualDistance);
//        }

//        public IEnumerable<TestCaseData> IntersectsBoundingRectangleTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), new BoundingRectangle(), true, Vector2.Zero).SetName(
//                        "The empty segment intersects the empty bounding box.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(-75, -75), new Vector2(75, -75)),
//                        new BoundingRectangle(new Vector2(), new Size2(50, 50)), false, Vector2.NaN).SetName(
//                        "A non-empty segment outside a non-empty bounding box does not intersect the bounding box.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(25, 0)), new BoundingRectangle(new Vector2(), new Size2(50, 50)),
//                        true, new Vector2(0, 0)).SetName(
//                        "A non-empty segment inside a non-empty bounding box intersects the bounding box.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(-100, 0), new Vector2(100, 0)),
//                        new BoundingRectangle(new Vector2(), new Size2(50, 50)),
//                        true, new Vector2(-50, 0)).SetName(
//                        "A non-empty segment crossing a non-empty bounding box intersects the bounding box.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(IntersectsBoundingRectangleTestCases))]
//        public void IntersectsBoundingRectangle(Segment2 segment, BoundingRectangle boundingRectangle, bool expectedResult,
//            Vector2 expectedIntersectionPoint)
//        {
//            Vector2 actualIntersectionPoint;
//            var actualResult = segment.Intersects(boundingRectangle, out actualIntersectionPoint);
//            Assert.Equal(expectedResult, actualResult);

//            if (actualResult)
//            {
//                Assert.Equal(expectedIntersectionPoint, actualIntersectionPoint);
//            }
//            else
//            {
//                Assert.IsTrue(float.IsNaN(actualIntersectionPoint.X));
//                Assert.IsTrue(float.IsNaN(actualIntersectionPoint.Y));
//            }
//        }

//        public IEnumerable<TestCaseData> EqualityTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), new Segment2(), true).SetName("Empty segments are equal.")
//                    ;
//                yield return
//                    new TestCaseData(
//                        new Segment2(new Vector2(0, 0), new Vector2(float.MaxValue, float.MinValue)),
//                        new Segment2(new Vector2(0, 0),
//                            new Vector2(float.MinValue, float.MaxValue)), false).SetName(
//                        "Two different non-empty segments are not equal.");
//                yield return
//                    new TestCaseData(
//                        new Segment2(new Vector2(0, 0), new Vector2(float.MinValue, float.MaxValue)),
//                        new Segment2(new Vector2(0, 0),
//                            new Vector2(float.MinValue, float.MaxValue)), true).SetName(
//                        "Two identical non-empty segments are equal.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(EqualityTestCases))]
//        public void Equality(Segment2 segment1, Segment2 segment2, bool expectedToBeEqual)
//        {
//            Assert.IsTrue(Equals(segment1, segment2) == expectedToBeEqual);
//            Assert.IsTrue(segment1 == segment2 == expectedToBeEqual);
//            Assert.IsFalse(segment1 == segment2 != expectedToBeEqual);
//            Assert.IsTrue(segment1.Equals(segment2) == expectedToBeEqual);

//            if (expectedToBeEqual)
//                Assert.Equal(segment1.GetHashCode(), segment2.GetHashCode());
//        }

//        public IEnumerable<TestCaseData> InequalityTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), null, false).SetName("A segment is not equal to a null object.");
//                yield return
//                    new TestCaseData(new Segment2(), new object(), false).SetName(
//                        "A segment is not equal to an instantiated object.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(InequalityTestCases))]
//        public void Inequality(Segment2 segment, object obj, bool expectedToBeEqual)
//        {
//            Assert.IsTrue(segment.Equals(obj) == expectedToBeEqual);
//        }

//        public IEnumerable<TestCaseData> HashCodeTestCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(), new Segment2(), true).SetName(
//                        "Two empty segments have the same hash code.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(50, 50)),
//                        new Segment2(new Vector2(0, 0), new Vector2(50, 50)), true).SetName(
//                        "Two indentical non-empty segments have the same hash code.");
//                yield return
//                    new TestCaseData(new Segment2(new Vector2(0, 0), new Vector2(50, 50)),
//                        new Segment2(new Vector2(50, 50), new Vector2(50, 50)), false).SetName(
//                        "Two different non-empty segments do not have the same hash code.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(HashCodeTestCases))]
//        public void HashCode(Segment2 segment1, Segment2 segment2, bool expectedThatHashCodesAreEqual)
//        {
//            var hashCode1 = segment1.GetHashCode();
//            var hashCode2 = segment2.GetHashCode();
//            if (expectedThatHashCodesAreEqual)
//                Assert.Equal(hashCode1, hashCode2);
//            else
//                Assert.AreNotEqual(hashCode1, hashCode2);
//        }

//        public IEnumerable<TestCaseData> StringCases
//        {
//            get
//            {
//                yield return
//                    new TestCaseData(new Segment2(),
//                        string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", new Vector2(),
//                            new Vector2())).SetName(
//                        "The empty segment has the expected string representation using the current culture.");
//                yield return new TestCaseData(new Segment2(new Vector2(5.1f, -5.123f), new Vector2(5.4f, -5.4123f)),
//                    string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", new Vector2(5.1f, -5.123f),
//                        new Vector2(5.4f, -5.4123f))).SetName(
//                    "A non-empty segment has the expected string representation using the current culture.");
//            }
//        }

//        [Fact]
//        [TestCaseSource(nameof(StringCases))]
//        public void String(Segment2 segment, string expectedString)
//        {
//            var actualString = segment.ToString();
//            Assert.Equal(expectedString, actualString);
//        }
//    }
//}
