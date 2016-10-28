using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Primitives;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class Ray2DTests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Vector2()).SetName(
                        "The degenerate ray has the expected position and direction.");
                yield return
                    new TestCaseData(new Point2(5, 5), new Vector2(15, 15)).SetName(
                        "A non-degenerate ray has the expected position and direction.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(Point2 position, Vector2 direction)
        {
            var ray = new Ray2D(position, direction);
            Assert.AreEqual(position, ray.Position);
            Assert.AreEqual(direction, ray.Direction);
        }

        public IEnumerable<TestCaseData> PositionDirectionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Ray2D(), new Point2(), new Vector2()).SetName(
                        "The degenerate ray has the expected position and direction.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(5, 5), new Vector2(15, 15)), new Point2(5, 5),
                            new Vector2(15, 15)).SetName
                        (
                            "A non-degenerate ray has the expected position and direction.");
            }
        }

        [Test]
        [TestCaseSource(nameof(PositionDirectionTestCases))]
        public void PositionDirection(Ray2D ray, Point2 expectedPosition, Vector2 expecetedDirection)
        {
            Assert.AreEqual(expectedPosition, ray.Position);
            Assert.AreEqual(expecetedDirection, ray.Direction);

            ray.Position.X = 10;
            ray.Position.Y = 10;
            Assert.AreEqual(new Point2(10, 10), ray.Position);

            ray.Direction.X = -10.123f;
            ray.Direction.Y = 10.123f;
            Assert.AreEqual(new Vector2(-10.123f, 10.123f), ray.Direction);
        }

        public IEnumerable<TestCaseData> IntersectsBoundingRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Ray2D(), new BoundingRectangle(), true, Point2.Zero, Point2.Zero).SetName(
                        "The degenerate ray intersects the empty bounding box.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(-75, -75), new Vector2(75, -75)),
                        new BoundingRectangle(new Point2(), new Size2(50, 50)), false, Point2.NaN, Point2.NaN).SetName(
                        "A non-degenerate ray that does not cross a non-empty bounding box does not intersect the bounding box.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(0, 0), new Vector2(25, 0)), new BoundingRectangle(new Point2(), new Size2(50, 50)),
                        true, new Point2(0, 0), new Point2(50, 0)).SetName(
                        "A non-degenerate ray starting from inside a non-empty bounding box intersects the bounding box.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(-100, 0), new Vector2(100, 0)),
                        new BoundingRectangle(new Point2(), new Size2(50, 50)),
                        true, new Point2(-50, 0), new Point2(50, 0)).SetName(
                        "A non-degenerate ray crossing a non-empty bounding box intersects the bounding box.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsBoundingRectangleTestCases))]
        public void IntersectsBoundingRectangle(Ray2D ray, BoundingRectangle boundingRectangle, bool expectedResult,
            Point2 firstExpectedIntersectionPoint, Point2 secondExpectedIntersectionPoint)
        {
            float rayNearDistance, rayFarDistance;
            var actualResult = ray.Intersects(boundingRectangle, out rayNearDistance, out rayFarDistance);
            Assert.AreEqual(expectedResult, actualResult);

            if (actualResult)
            {
                var firstActualIntersectionPoint = ray.Position + ray.Direction * rayNearDistance;
                Assert.AreEqual(firstExpectedIntersectionPoint, firstActualIntersectionPoint);
                var secondActualIntersectionPoint = ray.Position + ray.Direction * rayFarDistance;
                Assert.AreEqual(secondExpectedIntersectionPoint, secondActualIntersectionPoint);
            }
            else
            {
                Assert.IsTrue(float.IsNaN(rayNearDistance));
                Assert.IsTrue(float.IsNaN(rayFarDistance));
            }
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Ray2D(), new Ray2D(), true).SetName("Two degenerate rays are equal.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(float.MinValue, float.MaxValue),
                        new Vector2(float.MaxValue, float.MinValue)), new Ray2D(new Point2(float.MaxValue, float.MinValue),
                        new Vector2(float.MaxValue, float.MinValue)), false).SetName(
                        "Two different non-degenerate rays are not equal.");
                yield return
                    new TestCaseData(
                            new Ray2D(new Point2(float.MinValue, float.MaxValue),
                                new Vector2(float.MinValue, float.MaxValue)), new Ray2D(new Point2(float.MinValue, float.MaxValue),
                                new Vector2(float.MinValue, float.MaxValue)), true)
                        .SetName(
                            "Two identical non-degenerate rays are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(Ray2D ray1, Ray2D ray2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(ray1, ray2) == expectedToBeEqual);
            Assert.IsTrue(ray1 == ray2 == expectedToBeEqual);
            Assert.IsFalse(ray1 == ray2 != expectedToBeEqual);
            Assert.IsTrue(ray1.Equals(ray2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(ray1.GetHashCode(), ray2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Ray2D(), null, false).SetName("A ray is not equal to a null object.");
                yield return
                    new TestCaseData(new Ray2D(), new object(), false).SetName(
                        "A ray is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(Ray2D ray, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(ray.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Ray2D(), new Ray2D(), true).SetName(
                        "Two degenerate rays have the same hash code.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(50, 50), new Vector2(50, 50)),
                        new Ray2D(new Point2(50, 50), new Vector2(50, 50)), true).SetName(
                        "Two indentical non-zero points have the same hash code.");
                yield return
                    new TestCaseData(new Ray2D(new Point2(0, 0), new Vector2(50, 50)),
                        new Ray2D(new Point2(50, 50), new Vector2(50, 50)), false).SetName(
                        "Two different non-zero points do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(Ray2D ray1, Ray2D ray2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = ray1.GetHashCode();
            var hashCode2 = ray2.GetHashCode();
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
                    new TestCaseData(new Ray2D(),
                        string.Format(CultureInfo.CurrentCulture, "Position: {0}, Direction: {1}", new Point2(),
                            new Vector2())).SetName(
                        "The degenerate ray has the expected string representation using the current culture.");
                yield return new TestCaseData(new Ray2D(new Point2(5.1f, -5.123f), new Vector2(0, 1)),
                    string.Format(CultureInfo.CurrentCulture, "Position: {0}, Direction: {1}", new Point2(5.1f, -5.123f),
                        new Vector2(0, 1))).SetName(
                    "A non-degenerate ray has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(Ray2D ray, string expectedString)
        {
            var actualString = ray.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
