using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class BoundingRectangleTests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Vector2()).SetName(
                        "The empty bounding rectangle has the expected position and radii.");
                yield return
                    new TestCaseData(new Point2(5, 5), new Vector2(15, 15)).SetName(
                        "A non-empty bounding rectangle has the expected position and radii.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(Point2 centre, Vector2 radii)
        {
            var boundingRectangle = new BoundingRectangle(centre, radii);
            Assert.AreEqual(centre, boundingRectangle.Center);
            Assert.AreEqual(radii, boundingRectangle.HalfExtents);
        }

        public IEnumerable<TestCaseData> CreateFromMinimumMaximumTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Point2(), new BoundingRectangle()).SetName(
                        "The bounding rectangle created from the zero minimum point and zero maximum point is the empty bounding rectangle.")
                    ;
                yield return
                    new TestCaseData(new Point2(5, 5), new Point2(15, 15),
                        new BoundingRectangle(new Point2(10, 10), new Size2(5, 5))).SetName(
                        "The bounding rectangle created from the non-zero minimum point and the non-zero maximum point is the expected bounding rectangle.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromMinimumMaximumTestCases))]
        public void CreateFromMinimumMaximum(Point2 minimum, Point2 maximum, BoundingRectangle expectedBoundingRectangle)
        {
            var actualBoundingRectangle = BoundingRectangle.CreateFrom(minimum, maximum);
            Assert.AreEqual(expectedBoundingRectangle, actualBoundingRectangle);
        }

        public IEnumerable<TestCaseData> CreateFromPointsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(null, new BoundingRectangle()).SetName(
                        "The bounding rectangle created from null points is the empty bounding rectangle.");
                yield return
                    new TestCaseData(new Point2[0], new BoundingRectangle()).SetName(
                        "The bounding rectangle created from the empty set of points is the empty bounding rectangle.");
                yield return
                    new TestCaseData(
                        new[]
                        {
                            new Point2(5, 5), new Point2(10, 10), new Point2(15, 15), new Point2(-5, -5),
                            new Point2(-15, -15)
                        }, new BoundingRectangle(new Point2(0, 0), new Size2(15, 15))).SetName(
                        "The bounding rectangle created from a non-empty set of points is the expected bounding rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromPointsTestCases))]
        public void CreateFromPoints(Point2[] points, BoundingRectangle expectedBoundingRectangle)
        {
            var actualBoundingRectangle = BoundingRectangle.CreateFrom(points);
            Assert.AreEqual(expectedBoundingRectangle, actualBoundingRectangle);
        }

        public IEnumerable<TestCaseData> CreateFromTransformedTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), Matrix2D.Identity, new BoundingRectangle()).SetName(
                        "The bounding rectangle created from the empty bounding rectangle transformed by the identity matrix is the empty bounding rectangle.")
                    ;
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(20, 20)), Matrix2D.CreateScale(2), new BoundingRectangle(new Point2(0, 0), new Size2(40, 40))).SetName(
                        "The bounding rectangle created from a non-empty bounding rectangle transformed by a non-identity matrix is the expected bounding rectangle.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromTransformedTestCases))]
        public void CreateFromTransformed(BoundingRectangle boundingRectangle, Matrix2D transformMatrix,
            BoundingRectangle expectedBoundingRectangle)
        {
            var actualBoundingRectangle = BoundingRectangle.Transform(boundingRectangle, ref transformMatrix);
            Assert.AreEqual(expectedBoundingRectangle, actualBoundingRectangle);
        }

        public IEnumerable<TestCaseData> UnionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new BoundingRectangle(), new BoundingRectangle()).SetName(
                        "The union of two empty bounding rectangles is the empty bounding rectangle.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(15, 15)),
                            new BoundingRectangle(new Point2(20, 20), new Size2(40, 40)), new BoundingRectangle(new Point2(20, 20), new Size2(40, 40)))
                        .SetName(
                            "The union of two non-empty bounding rectangles is the expected bounding rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(UnionTestCases))]
        public void Union(BoundingRectangle boundingRectangle1, BoundingRectangle boundingRectangle2, BoundingRectangle expectedBoundingRectangle)
        {
            Assert.AreEqual(expectedBoundingRectangle, boundingRectangle1.Union(boundingRectangle2));
            Assert.AreEqual(expectedBoundingRectangle, BoundingRectangle.Union(boundingRectangle1, boundingRectangle2));
        }

        public IEnumerable<TestCaseData> IntersectionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new BoundingRectangle(), new BoundingRectangle()).SetName(
                        "The intersection of two empty bounding rectangles is the empty bounding box.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(-10, -10), new Size2(15, 15)),
                        new BoundingRectangle(new Point2(20, 20), new Size2(40, 40)),
                        new BoundingRectangle(new Point2(-7.5f, -7.5f), new Size2(12.5f, 12.5f))).SetName(
                        "The intersection of two overlapping non-empty bounding rectangles is the expected bounding rectangle.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(-30, -30), new Size2(15, 15)),
                        new BoundingRectangle(new Point2(20, 20), new Size2(10, 10)),
                        BoundingRectangle.Empty).SetName(
                        "The intersection of two non-overlapping non-empty bounding rectangles is the empty bounding rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectionTestCases))]
        public void Intersection(BoundingRectangle boundingRectangle1, BoundingRectangle boundingRectangle2,
            BoundingRectangle? expectedBoundingRectangle)
        {
            Assert.AreEqual(expectedBoundingRectangle, boundingRectangle1.Intersection(boundingRectangle2));
            Assert.AreEqual(expectedBoundingRectangle, BoundingRectangle.Intersection(boundingRectangle1, boundingRectangle2));
        }

        public IEnumerable<TestCaseData> IntersectsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new BoundingRectangle(), true).SetName(
                        "Two empty bounding rectangles intersect.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(-10, -10), new Size2(15, 15)),
                        new BoundingRectangle(new Point2(20, 20), new Size2(40, 40)), true).SetName(
                        "Two overlapping non-empty bounding rectangles intersect.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(-40, -50), new Size2(15, 15)),
                        new BoundingRectangle(new Point2(20, 20), new Size2(15, 15)), false).SetName(
                        "Two non-overlapping non-empty bounding rectangles do not intersect.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsTestCases))]
        public void Intersects(BoundingRectangle boundingRectangle1, BoundingRectangle boundingRectangle2, bool expectedToIntersect)
        {
            Assert.AreEqual(expectedToIntersect, boundingRectangle1.Intersects(boundingRectangle2));
            Assert.AreEqual(expectedToIntersect, BoundingRectangle.Intersects(boundingRectangle1, boundingRectangle2));
        }

        public IEnumerable<TestCaseData> ContainsPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new Point2(), true).SetName(
                        "The empty bounding rectangle contains the zero point.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(15, 15)), new Point2(-15, -15), true)
                        .SetName(
                            "A non-empty bounding rectangle contains a point inside it.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(15, 15)), new Point2(-16, 15), false)
                        .SetName(
                            "A non-empty bounding rectangle does not contain a point outside it.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ContainsPointTestCases))]
        public void ContainsPoint(BoundingRectangle boundingRectangle, Point2 point, bool expectedToContainPoint)
        {
            Assert.AreEqual(expectedToContainPoint, boundingRectangle.Contains(point));
            Assert.AreEqual(expectedToContainPoint, BoundingRectangle.Contains(boundingRectangle, point));
        }

        public IEnumerable<TestCaseData> ClosestPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new Point2(), new Point2()).SetName(
                        "The closest point on the empty bounding rectangle to the zero point is the zero point.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Point2(50, 50)), new Point2(25, 25),
                        new Point2(25, 25)).SetName(
                        "The closest point on a non-empty bounding rectangle to a point which is inside the bounding rectangle is that point.")
                    ;
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Point2(50, 50)), new Point2(400, 0),
                        new Point2(50, 0)).SetName(
                        "The closest point on a non-empty bounding rectangle to a point which is outside the bounding rectangle is the expected point.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(ClosestPointTestCases))]
        public void ClosestPoint(BoundingRectangle boundingRectangle, Point2 point, Point2 expectedClosestPoint)
        {
            var actualClosestPoint = boundingRectangle.ClosestPointTo(point);
            Assert.AreEqual(expectedClosestPoint, actualClosestPoint);
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new BoundingRectangle(), true).SetName(
                        "Empty bounding rectangles are equal.")
                    ;
                yield return
                    new TestCaseData(
                        new BoundingRectangle(new Point2(0, 0), new Size2(float.MaxValue, float.MinValue)),
                        new BoundingRectangle(new Point2(0, 0),
                            new Point2(float.MinValue, float.MaxValue)), false).SetName(
                        "Two different non-empty bounding rectangles are not equal.");
                yield return
                    new TestCaseData(
                        new BoundingRectangle(new Point2(0, 0), new Size2(float.MinValue, float.MaxValue)),
                        new BoundingRectangle(new Point2(0, 0),
                            new Size2(float.MinValue, float.MaxValue)), true).SetName(
                        "Two identical non-empty bounding rectangles are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(BoundingRectangle boundingRectangle1, BoundingRectangle boundingRectangle2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(boundingRectangle1, boundingRectangle2) == expectedToBeEqual);
            Assert.IsTrue(boundingRectangle1 == boundingRectangle2 == expectedToBeEqual);
            Assert.IsFalse(boundingRectangle1 == boundingRectangle2 != expectedToBeEqual);
            Assert.IsTrue(boundingRectangle1.Equals(boundingRectangle2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(boundingRectangle1.GetHashCode(), boundingRectangle2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), null, false).SetName(
                        "A bounding rectangle is not equal to a null object.");
                yield return
                    new TestCaseData(new BoundingRectangle(), new object(), false).SetName(
                        "A bounding rectangle is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(BoundingRectangle boundingRectangle, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(boundingRectangle.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new BoundingRectangle(), true).SetName(
                        "Two empty bounding rectangles have the same hash code.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(50, 50)),
                        new BoundingRectangle(new Point2(0, 0), new Size2(50, 50)), true).SetName(
                        "Two indentical non-empty bounding rectangles have the same hash code.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(0, 0), new Size2(50, 50)),
                        new BoundingRectangle(new Point2(50, 50), new Size2(50, 50)), false).SetName(
                        "Two different non-empty bounding rectangles do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(BoundingRectangle boundingRectangle1, BoundingRectangle boundingRectangle2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = boundingRectangle1.GetHashCode();
            var hashCode2 = boundingRectangle2.GetHashCode();
            if (expectedThatHashCodesAreEqual)
                Assert.AreEqual(hashCode1, hashCode2);
            else
                Assert.AreNotEqual(hashCode1, hashCode2);
        }

        public IEnumerable<TestCaseData> ToRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(), new Rectangle()).SetName(
                        "The empty bounding rectangle point converted to a rectangle is the empty rectangle.");
                yield return
                    new TestCaseData(new BoundingRectangle(new Point2(25, 25), new Size2(25, 25)),
                        new Rectangle(0, 0, 50, 50)).SetName(
                        "A non-empty bounding rectangle converted to a rectangle is the expected rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ToRectangleTestCases))]
        public void ToRectangle(BoundingRectangle boundingRectangle, Rectangle expectedRectangle)
        {
            var actualRectangle = (Rectangle)boundingRectangle;
            Assert.AreEqual(expectedRectangle, actualRectangle);
        }

        public IEnumerable<TestCaseData> FromRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Rectangle(), new BoundingRectangle()).SetName(
                        "The empty rectangle converted to a bounding rectangle is the empty bounding rectangle.");
                yield return
                    new TestCaseData(new Rectangle(0, 0, 50, 50),
                        new BoundingRectangle(new Point2(25, 25), new Size2(25, 25))).SetName(
                        "A non-empty rectangle converted to a bounding rectangle is the expected bounding rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(FromRectangleTestCases))]
        public void FromRectangle(Rectangle rectangle, BoundingRectangle expectedBoundingRectangle)
        {
            var actualBoundingRectangle = (BoundingRectangle)rectangle;
            Assert.AreEqual(expectedBoundingRectangle, actualBoundingRectangle);
        }

        public IEnumerable<TestCaseData> StringCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingRectangle(),
                        string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radii: {1}", new Point2(),
                            new Vector2())).SetName(
                        "The empty bounding rectangle has the expected string representation using the current culture.");
                yield return new TestCaseData(new BoundingRectangle(new Point2(5.1f, -5.123f), new Size2(5.4f, -5.4123f)),
                    string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radii: {1}", new Point2(5.1f, -5.123f),
                        new Vector2(5.4f, -5.4123f))).SetName(
                    "A non-empty bounding rectangle has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(BoundingRectangle boundingRectangle, string expectedString)
        {
            var actualString = boundingRectangle.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
