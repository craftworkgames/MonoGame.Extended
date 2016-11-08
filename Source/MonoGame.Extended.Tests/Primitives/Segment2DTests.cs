using System.Collections.Generic;
using System.Globalization;
using MonoGame.Extended.Primitives;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    public class Segment2DTests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Point2()).SetName(
                        "The empty segment has expected starting and ending points.");
                yield return
                    new TestCaseData(
                        new Point2(float.MaxValue, float.MinValue),
                        new Point2(int.MaxValue, int.MinValue)).SetName(
                        "A non-empty segment has the expected starting and ending points.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(Point2 startingPoint, Point2 endingPoint)
        {
            var segment = new Segment2D(startingPoint, endingPoint);
            Assert.AreEqual(startingPoint, segment.Start);
            Assert.AreEqual(endingPoint, segment.End);
        }

        public IEnumerable<TestCaseData> ClosestPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), new Point2(), new Point2()).SetName(
                        "The closest point on the empty segment to the zero point is the zero point.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(200, 0)), new Point2(-100, 200),
                        new Point2(0, 0)).SetName(
                        "The closest point on a non-empty segment to a point which is projected beyond the start of the segment is the segment's starting point.")
                    ;
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(200, 0)), new Point2(400, 200),
                        new Point2(200, 0)).SetName(
                        "The closest point on a non-empty segment to a point which is projected beyond the end of the segment is the segment's ending point.")
                    ;
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(200, 0)), new Point2(100, 200),
                        new Point2(100, 0)).SetName(
                        "The closest point on a non-empty segment to a point which is projected inside the segment is the projected point.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(ClosestPointTestCases))]
        public void ClosestPoint(Segment2D segment, Point2 point, Point2 expectedClosestPoint)
        {
            var actualClosestPoint = segment.ClosestPointTo(point);
            Assert.AreEqual(expectedClosestPoint, actualClosestPoint);
        }

        public IEnumerable<TestCaseData> SquaredDistanceToPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), new Point2(), 0).SetName(
                        "The squared distance of the zero point to the empty segment is 0.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(20, 0)), new Point2(-10, 20), 500)
                        .SetName(
                            "The squared distance of a point projected beyond the start of a non-empty segment is the squared distance from the segment's starting point to the point.")
                    ;
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(20, 0)), new Point2(40, 20), 400)
                        .SetName(
                            "The squared distance of a point projected beyond the end of a non-empty segment is the squared distance from the segment's ending point to the point.")
                    ;
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(20, 0)), new Point2(10, 25), 625).SetName
                    (
                        "The squared distance of a point projected inside a non-empty segment is the squared distance from the projected point to the point.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(SquaredDistanceToPointTestCases))]
        public void SquaredDistanceToPoint(Segment2D segment, Point2 point,
            float expectedDistance)
        {
            var actualDistance = segment.SquaredDistanceTo(point);
            Assert.AreEqual(expectedDistance, actualDistance);
        }

        public IEnumerable<TestCaseData> IntersectsBoundingRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), new BoundingRectangle(), true, Point2.Zero).SetName(
                        "The empty segment intersects the empty bounding box.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(-75, -75), new Point2(75, -75)),
                        new BoundingRectangle(new Point2(), new Size2(50, 50)), false, Point2.NaN).SetName(
                        "A non-empty segment outside a non-empty bounding box does not intersect the bounding box.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(25, 0)), new BoundingRectangle(new Point2(), new Size2(50, 50)),
                        true, new Point2(0, 0)).SetName(
                        "A non-empty segment inside a non-empty bounding box intersects the bounding box.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(-100, 0), new Point2(100, 0)),
                        new BoundingRectangle(new Point2(), new Size2(50, 50)),
                        true, new Point2(-50, 0)).SetName(
                        "A non-empty segment crossing a non-empty bounding box intersects the bounding box.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsBoundingRectangleTestCases))]
        public void IntersectsBoundingRectangle(Segment2D segment, BoundingRectangle boundingRectangle, bool expectedResult,
            Point2 expectedIntersectionPoint)
        {
            Point2 actualIntersectionPoint;
            var actualResult = segment.Intersects(boundingRectangle, out actualIntersectionPoint);
            Assert.AreEqual(expectedResult, actualResult);

            if (actualResult)
            {
                Assert.AreEqual(expectedIntersectionPoint, actualIntersectionPoint);
            }
            else
            {
                Assert.IsTrue(float.IsNaN(actualIntersectionPoint.X));
                Assert.IsTrue(float.IsNaN(actualIntersectionPoint.Y));
            }
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), new Segment2D(), true).SetName("Empty segments are equal.")
                    ;
                yield return
                    new TestCaseData(
                        new Segment2D(new Point2(0, 0), new Point2(float.MaxValue, float.MinValue)),
                        new Segment2D(new Point2(0, 0),
                            new Point2(float.MinValue, float.MaxValue)), false).SetName(
                        "Two different non-empty segments are not equal.");
                yield return
                    new TestCaseData(
                        new Segment2D(new Point2(0, 0), new Point2(float.MinValue, float.MaxValue)),
                        new Segment2D(new Point2(0, 0),
                            new Point2(float.MinValue, float.MaxValue)), true).SetName(
                        "Two identical non-empty segments are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(Segment2D segment1, Segment2D segment2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(segment1, segment2) == expectedToBeEqual);
            Assert.IsTrue(segment1 == segment2 == expectedToBeEqual);
            Assert.IsFalse(segment1 == segment2 != expectedToBeEqual);
            Assert.IsTrue(segment1.Equals(segment2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(segment1.GetHashCode(), segment2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), null, false).SetName("A segment is not equal to a null object.");
                yield return
                    new TestCaseData(new Segment2D(), new object(), false).SetName(
                        "A segment is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(Segment2D segment, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(segment.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(), new Segment2D(), true).SetName(
                        "Two empty segments have the same hash code.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(50, 50)),
                        new Segment2D(new Point2(0, 0), new Point2(50, 50)), true).SetName(
                        "Two indentical non-empty segments have the same hash code.");
                yield return
                    new TestCaseData(new Segment2D(new Point2(0, 0), new Point2(50, 50)),
                        new Segment2D(new Point2(50, 50), new Point2(50, 50)), false).SetName(
                        "Two different non-empty segments do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(Segment2D segment1, Segment2D segment2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = segment1.GetHashCode();
            var hashCode2 = segment2.GetHashCode();
            if (expectedThatHashCodesAreEqual)
                Assert.AreEqual(hashCode1, hashCode2);
            else
                Assert.AreNotEqual(hashCode1, hashCode2);
        }

        public IEnumerable<TestCaseData> StringCases
        {
            get
            {
                yield return
                    new TestCaseData(new Segment2D(),
                        string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", new Point2(),
                            new Point2())).SetName(
                        "The empty segment has the expected string representation using the current culture.");
                yield return new TestCaseData(new Segment2D(new Point2(5.1f, -5.123f), new Point2(5.4f, -5.4123f)),
                    string.Format(CultureInfo.CurrentCulture, "{0} -> {1}", new Point2(5.1f, -5.123f),
                        new Point2(5.4f, -5.4123f))).SetName(
                    "A non-empty segment has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(Segment2D segment, string expectedString)
        {
            var actualString = segment.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
