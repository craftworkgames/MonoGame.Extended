using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class CircleFTests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), 0.0f).SetName(
                        "The empty circle has the expected position and radius.");
                yield return
                    new TestCaseData(new Point2(5, 5), 15f).SetName(
                        "A non-empty circle has the expected position and radius.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(Point2 centre, float radius)
        {
            var circle = new CircleF(centre, radius);
            Assert.AreEqual(centre, circle.Center);
            Assert.AreEqual(radius, circle.Radius);
        }

        public IEnumerable<TestCaseData> CreateFromMinimumMaximumTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Point2(), new CircleF()).SetName(
                        "The bounding circle created from the zero minimum point and zero maximum point is the empty bounding circle.")
                    ;
                yield return
                    new TestCaseData(new Point2(5, 5), new Point2(15, 15),
                        new CircleF(new Point2(10, 10), 5f)).SetName(
                        "The bounding circle created from the non-zero minimum point and the non-zero maximum point is the expected bounding circle.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromMinimumMaximumTestCases))]
        public void CreateFromMinimumMaximum(Point2 minimum, Point2 maximum, CircleF expectedBoundingCircle)
        {
            var actualBoundingCircle = CircleF.CreateFrom(minimum, maximum);
            Assert.AreEqual(expectedBoundingCircle, actualBoundingCircle);
        }

        public IEnumerable<TestCaseData> CreateFromPointsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(null, new CircleF()).SetName(
                        "The bounding circle created from null points is the empty bounding circle.");
                yield return
                    new TestCaseData(new Point2[0], new CircleF()).SetName(
                        "The bounding circle created from the empty set of points is the empty bounding circle.");
                yield return
                    new TestCaseData(
                        new[]
                        {
                            new Point2(5, 5), new Point2(10, 10), new Point2(15, 15), new Point2(-5, -5),
                            new Point2(-15, -15)
                        }, new CircleF(new Point2(0, 0), 15)).SetName(
                        "The bounding circle created from a non-empty set of points is the expected bounding circle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromPointsTestCases))]
        public void CreateFromPoints(Point2[] points, CircleF expectedCircle)
        {
            var actualCircle = CircleF.CreateFrom(points);
            Assert.AreEqual(expectedCircle, actualCircle);
        }

        public IEnumerable<TestCaseData> IntersectsCircleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new CircleF(), true).SetName(
                        "Two empty circles intersect.");
                yield return
                    new TestCaseData(new CircleF(new Point2(-10, -10), 15),
                        new CircleF(new Point2(20, 20), 40), true).SetName(
                        "Two overlapping non-empty circles intersect.");
                yield return
                    new TestCaseData(new CircleF(new Point2(-40, -50), 15),
                        new CircleF(new Point2(20, 20), 15), false).SetName(
                        "Two non-overlapping non-empty circles do not intersect.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsCircleTestCases))]
        public void Intersects(CircleF circle, CircleF circle2, bool expectedToIntersect)
        {
            Assert.AreEqual(expectedToIntersect, circle.Intersects(circle2));
            Assert.AreEqual(expectedToIntersect, CircleF.Intersects(circle, circle2));
        }

        public IEnumerable<TestCaseData> IntersectsRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new RectangleF(), true).SetName(
                        "The empty circle and the empty rectangle intersect.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 15),
                        new RectangleF(new Point2(0, 0), new Size2(40, 40)), true).SetName(
                        "The non-empty circle and a non-empty overlapping rectangle intersect.");
                yield return
                    new TestCaseData(new CircleF(new Point2(-40, -50), 15),
                        new RectangleF(new Point2(20, 20), new Size2(15, 15)), false).SetName(
                        "The non-empty circle and a non-empty non-overlapping rectangle do not intersect.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsRectangleTestCases))]
        public void Intersects(CircleF circle, RectangleF rectangle, bool expectedToIntersect)
        {
            Assert.AreEqual(expectedToIntersect, circle.Intersects((BoundingRectangle)rectangle));
            Assert.AreEqual(expectedToIntersect, CircleF.Intersects(circle, (BoundingRectangle)rectangle));
        }

        public IEnumerable<TestCaseData> ContainsPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new Point2(), true).SetName(
                        "The empty circle contains the zero point.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 15), new Point2(-15, -15), true)
                        .SetName(
                            "A non-empty circle contains a point inside it.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 15), new Point2(-16, 15), false)
                        .SetName(
                            "A non-empty circle does not contain a point outside it.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ContainsPointTestCases))]
        public void ContainsPoint(CircleF circle, Point2 point, bool expectedToContainPoint)
        {
            Assert.AreEqual(expectedToContainPoint, circle.Contains(point));
            Assert.AreEqual(expectedToContainPoint, CircleF.Contains(circle, point));
        }

        public IEnumerable<TestCaseData> ClosestPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new Point2(), new Point2()).SetName(
                        "The closest point on the empty circle to the zero point is the zero point.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 50), new Point2(25, 25),
                        new Point2(25, 25)).SetName(
                        "The closest point on a non-empty circle to a point which is inside the circle is that point.")
                    ;
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 50), new Point2(400, 0),
                        new Point2(50, 0)).SetName(
                        "The closest point on a non-empty circle to a point which is outside the circle is the expected point.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(ClosestPointTestCases))]
        public void ClosestPoint(CircleF circle, Point2 point, Point2 expectedClosestPoint)
        {
            var actualClosestPoint = circle.ClosestPointTo(point);
            Assert.AreEqual(expectedClosestPoint, actualClosestPoint);
        }

        public IEnumerable<TestCaseData> BoundaryPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), 0.0f, new Point2()).SetName(
                        "The boundary point on the empty circle at an angle is the zero point.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 50), MathHelper.PiOver2,
                        new Point2(0, 50)).SetName(
                        "The boundary point on a non-empty circle at an angle is the expected point.");
            }
        }

        [Test]
        [TestCaseSource(nameof(BoundaryPointTestCases))]
        public void BoundaryPointAt(CircleF circle, float angle, Point2 expectedPoint)
        {
            var actualPoint = circle.BoundaryPointAt(angle);
            AssertExtensions.AreApproximatelyEqual(expectedPoint, actualPoint);
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new CircleF(), true).SetName(
                        "Empty circles are equal.")
                    ;
                yield return
                    new TestCaseData(
                        new CircleF(new Point2(0, 0), float.MaxValue),
                        new CircleF(new Point2(0, 0), float.MinValue), false).SetName(
                        "Two different non-empty circles are not equal.");
                yield return
                    new TestCaseData(
                        new CircleF(new Point2(0, 0), float.MinValue),
                        new CircleF(new Point2(0, 0), float.MinValue), true).SetName(
                        "Two identical non-empty circles are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(CircleF circle1, CircleF circle2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(circle1, circle2) == expectedToBeEqual);
            Assert.IsTrue(circle1 == circle2 == expectedToBeEqual);
            Assert.IsFalse(circle1 == circle2 != expectedToBeEqual);
            Assert.IsTrue(circle1.Equals(circle2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(circle1.GetHashCode(), circle2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), null, false).SetName(
                        "A circle is not equal to a null object.");
                yield return
                    new TestCaseData(new CircleF(), new object(), false).SetName(
                        "A circle is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(CircleF circle, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(circle.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new CircleF(), true).SetName(
                        "Two empty circles have the same hash code.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 50),
                        new CircleF(new Point2(0, 0), 50), true).SetName(
                        "Two indentical non-empty circles have the same hash code.");
                yield return
                    new TestCaseData(new CircleF(new Point2(0, 0), 50),
                        new CircleF(new Point2(50, 50), 50), false).SetName(
                        "Two different non-empty circles do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(CircleF circle1, CircleF circle2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = circle1.GetHashCode();
            var hashCode2 = circle2.GetHashCode();
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
                    new TestCaseData(new CircleF(), new Rectangle()).SetName(
                        "The empty circle converted to a rectangle is the empty integer rectangle.");
                yield return
                    new TestCaseData(new CircleF(new Point2(25, 25), 25),
                        new Rectangle(0, 0, 50, 50)).SetName(
                        "A non-empty circle converted to a rectangle is the expected integer rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ToRectangleTestCases))]
        public void ToRectangle(CircleF circle, Rectangle expectedRectangle)
        {
            var actualRectangle = (Rectangle)circle;
            Assert.AreEqual(expectedRectangle, actualRectangle);
        }

        public IEnumerable<TestCaseData> ToRectangleFTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(), new RectangleF()).SetName(
                        "The empty circle converted to a rectangle is the empty float rectangle.");
                yield return
                    new TestCaseData(new CircleF(new Point2(25, 25), 25),
                        new RectangleF(0, 0, 50, 50)).SetName(
                        "A non-empty circle converted to a rectangle is the expected float rectangle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ToRectangleFTestCases))]
        public void ToRectangleF(CircleF circle, RectangleF expectedRectangle)
        {
            var actualRectangle = (RectangleF)circle;
            Assert.AreEqual(expectedRectangle, actualRectangle);
        }

        public IEnumerable<TestCaseData> FromRectangleTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Rectangle(), new CircleF()).SetName(
                        "The empty rectangle converted to a circle is the empty circle.");
                yield return
                    new TestCaseData(new Rectangle(0, 0, 50, 50),
                        new CircleF(new Point2(25, 25), 25)).SetName(
                        "A non-empty rectangle converted to a circle is the expected circle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(FromRectangleTestCases))]
        public void FromRectangle(Rectangle rectangle, CircleF expectedCircle)
        {
            var actualCircle = (CircleF)rectangle;
            Assert.AreEqual(expectedCircle, actualCircle);
        }

        public IEnumerable<TestCaseData> FromRectangleFTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new RectangleF(), new CircleF()).SetName(
                        "The empty rectangle converted to a circle is the empty circle.");
                yield return
                    new TestCaseData(new RectangleF(0, 0, 50, 50),
                        new CircleF(new Point2(25, 25), 25)).SetName(
                        "A non-empty rectangle converted to a circle is the expected circle.");
            }
        }

        [Test]
        [TestCaseSource(nameof(FromRectangleFTestCases))]
        public void FromRectangleF(RectangleF rectangle, CircleF expectedCircle)
        {
            var actualCircle = (CircleF)rectangle;
            Assert.AreEqual(expectedCircle, actualCircle);
        }

        public IEnumerable<TestCaseData> StringCases
        {
            get
            {
                yield return
                    new TestCaseData(new CircleF(),
                        string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radius: {1}", new Point2(),
                            0)).SetName(
                        "The empty circle has the expected string representation using the current culture.");
                yield return new TestCaseData(new CircleF(new Point2(5.1f, -5.123f), 5.4f),
                    string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radius: {1}", new Point2(5.1f, -5.123f),
                        5.4f)).SetName(
                    "A non-empty circle has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(CircleF circle, string expectedString)
        {
            var actualString = circle.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
