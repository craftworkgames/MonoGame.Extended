using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Primitives;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class BoundingBox2DTests
    {
        public IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Size2()).SetName(
                        "The empty bounding box has the expected position and direction.");
                yield return
                    new TestCaseData(new Point2(5, 5), new Size2(15, 15)).SetName(
                        "A non-empty bounding box has the expected position and direction.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ConstructorTestCases))]
        public void Constructor(Point2 centre, Size2 radius)
        {
            var boundingBox = new BoundingBox2D(centre, radius);
            Assert.AreEqual(centre, boundingBox.Centre);
            Assert.AreEqual(radius, boundingBox.Radius);
        }

        public IEnumerable<TestCaseData> CreateFromMinimumMaximumTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new Point2(), new Point2(), new BoundingBox2D()).SetName(
                        "The bounding box created from the zero minimum point and zero maximum point is the empty bounding box.")
                    ;
                yield return
                    new TestCaseData(new Point2(5, 5), new Point2(15, 15),
                        new BoundingBox2D(new Point2(10, 10), new Size2(5, 5))).SetName(
                        "The bounding box created from the non-zero minimum point and the non-zero maximum point is the expected bounding box.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromMinimumMaximumTestCases))]
        public void CreateFromMinimumMaximum(Point2 minimum, Point2 maximum, BoundingBox2D expectedBoundingBox)
        {
            var actualBoundingBox = BoundingBox2D.CreateFromMinimumMaximum(minimum, maximum);
            Assert.AreEqual(expectedBoundingBox, actualBoundingBox);
        }

        public IEnumerable<TestCaseData> CreateFromPointsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(null, new BoundingBox2D()).SetName(
                        "The bounding box created from null points is the empty bounding box.");
                yield return
                    new TestCaseData(new Point2[0], new BoundingBox2D()).SetName(
                        "The bounding box created from the empty set of points is the empty bounding box.");
                yield return
                    new TestCaseData(
                        new[]
                        {
                            new Point2(5, 5), new Point2(10, 10), new Point2(15, 15), new Point2(-5, -5),
                            new Point2(-15, -15)
                        }, new BoundingBox2D(new Point2(0, 0), new Size2(15, 15))).SetName(
                        "The bounding box created from a non-empty set of points is the expected bounding box.");
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromPointsTestCases))]
        public void CreateFromPoints(Point2[] points, BoundingBox2D expectedBoundingBox)
        {
            var actualBoundingBox = BoundingBox2D.CreateFromPoints(points);
            Assert.AreEqual(expectedBoundingBox, actualBoundingBox);
        }

        public IEnumerable<TestCaseData> CreateFromTransformedTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), Matrix2D.Identity, new BoundingBox2D()).SetName(
                        "The bounding box created from the empty bounding box transformed by the identity matrix is the empty bounding box.")
                    ;
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(20, 20)), Matrix2D.CreateScale(2), new BoundingBox2D(new Point2(0, 0), new Size2(40, 40))).SetName(
                        "The bounding box created from a non-empty bounding box transformed by a non-identity matrix is the expected bounding box.")
                    ;
            }
        }

        [Test]
        [TestCaseSource(nameof(CreateFromTransformedTestCases))]
        public void CreateFromTransformed(BoundingBox2D boundingBox, Matrix2D transformMatrix,
            BoundingBox2D expectedBoundingBox)
        {
            var actualBoundingBox = BoundingBox2D.CreateFromTransformedBoundingBox(boundingBox, ref transformMatrix);
            Assert.AreEqual(expectedBoundingBox, actualBoundingBox);
        }

        public IEnumerable<TestCaseData> UnionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new BoundingBox2D(), new BoundingBox2D()).SetName(
                        "The union of two empty bounding boxes is the empty bounding box.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(15, 15)),
                            new BoundingBox2D(20, 20, 40, 40), new BoundingBox2D(new Point2(20, 20), new Size2(40, 40)))
                        .SetName(
                            "The union of two non-empty bounding boxes is the expected bounding box.");
            }
        }

        [Test]
        [TestCaseSource(nameof(UnionTestCases))]
        public void Union(BoundingBox2D boundingBox1, BoundingBox2D boundingBox2, BoundingBox2D expectedBoundingBox)
        {
            Assert.AreEqual(expectedBoundingBox, boundingBox1.Union(boundingBox2));
            Assert.AreEqual(expectedBoundingBox, BoundingBox2D.Union(boundingBox1, boundingBox2));
        }

        public IEnumerable<TestCaseData> IntersectionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new BoundingBox2D(), new BoundingBox2D()).SetName(
                        "The intersection of two empty bounding boxes is the empty bounding box.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(-10, -10), new Size2(15, 15)),
                        new BoundingBox2D(20, 20, 40, 40),
                        new BoundingBox2D(new Point2(-7.5f, -7.5f), new Size2(12.5f, 12.5f))).SetName(
                        "The intersection of two overlapping non-empty bounding boxes is the expected bounding box.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(-30, -30), new Size2(15, 15)),
                        new BoundingBox2D(20, 20, 10, 10),
                        null).SetName(
                        "The intersection of two non-overlapping non-empty bounding boxes is null.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectionTestCases))]
        public void Intersection(BoundingBox2D boundingBox1, BoundingBox2D boundingBox2,
            BoundingBox2D? expectedBoundingBox)
        {
            Assert.AreEqual(expectedBoundingBox, boundingBox1.Intersection(boundingBox2));
            Assert.AreEqual(expectedBoundingBox, BoundingBox2D.Intersection(boundingBox1, boundingBox2));
        }

        public IEnumerable<TestCaseData> IntersectsTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new BoundingBox2D(), true).SetName(
                        "Two empty bounding boxes intersect.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(-10, -10), new Size2(15, 15)),
                        new BoundingBox2D(20, 20, 40, 40),
                        new BoundingBox2D(new Point2(-7.5f, -7.5f), new Size2(12.5f, 12.5f)), true).SetName(
                        "Two overlapping non-empty bounding boxes intersect.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(-40, -50), new Size2(15, 15)),
                        new BoundingBox2D(20, 20, 15, 15), false).SetName(
                        "Two non-overlapping non-empty bounding boxes do not intersect.");
            }
        }

        [Test]
        [TestCaseSource(nameof(IntersectsTestCases))]
        public void Intersects(BoundingBox2D boundingBox1, BoundingBox2D boundingBox2, bool expectedToIntersect)
        {
            Assert.AreEqual(expectedToIntersect, boundingBox1.Intersects(boundingBox2));
            Assert.AreEqual(expectedToIntersect, BoundingBox2D.Intersects(boundingBox1, boundingBox2));
        }

        public IEnumerable<TestCaseData> ContainsPointTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new Point2(), true).SetName(
                        "The empty bounding box contains the zero point.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(15, 15)), new Point2(-15, -15), true)
                        .SetName(
                            "A non-empty bounding box contains a point inside it.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(15, 15)), new Point2(-16, 15), false)
                        .SetName(
                            "A non-empty bounding box does not contain a point outside it.");
            }
        }

        [Test]
        [TestCaseSource(nameof(ContainsPointTestCases))]
        public void ContainsPoint(BoundingBox2D boundingBox, Point2 point, bool expectedToContainPoint)
        {
            Assert.AreEqual(expectedToContainPoint, boundingBox.Contains(point));
            Assert.AreEqual(expectedToContainPoint, BoundingBox2D.Contains(boundingBox, point));
        }

        public IEnumerable<TestCaseData> EqualityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new BoundingBox2D(), true).SetName(
                        "Empty bounding boxes are equal.")
                    ;
                yield return
                    new TestCaseData(
                        new BoundingBox2D(new Point2(0, 0), new Size2(float.MaxValue, float.MinValue)),
                        new BoundingBox2D(new Point2(0, 0),
                            new Point2(float.MinValue, float.MaxValue)), false).SetName(
                        "Two different non-empty segments are not equal.");
                yield return
                    new TestCaseData(
                        new BoundingBox2D(new Point2(0, 0), new Size2(float.MinValue, float.MaxValue)),
                        new BoundingBox2D(new Point2(0, 0),
                            new Size2(float.MinValue, float.MaxValue)), true).SetName(
                        "Two identical non-empty segments are equal.");
            }
        }

        [Test]
        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(BoundingBox2D boundingBox1, BoundingBox2D boundingBox2, bool expectedToBeEqual)
        {
            Assert.IsTrue(Equals(boundingBox1, boundingBox2) == expectedToBeEqual);
            Assert.IsTrue(boundingBox1 == boundingBox2 == expectedToBeEqual);
            Assert.IsFalse(boundingBox1 == boundingBox2 != expectedToBeEqual);
            Assert.IsTrue(boundingBox1.Equals(boundingBox2) == expectedToBeEqual);

            if (expectedToBeEqual)
                Assert.AreEqual(boundingBox1.GetHashCode(), boundingBox2.GetHashCode());
        }

        public IEnumerable<TestCaseData> InequalityTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), null, false).SetName(
                        "A bounding box is not equal to a null object.");
                yield return
                    new TestCaseData(new BoundingBox2D(), new object(), false).SetName(
                        "A bounding box is not equal to an instantiated object.");
            }
        }

        [Test]
        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(BoundingBox2D boundingBox, object obj, bool expectedToBeEqual)
        {
            Assert.IsTrue(boundingBox.Equals(obj) == expectedToBeEqual);
        }

        public IEnumerable<TestCaseData> HashCodeTestCases
        {
            get
            {
                yield return
                    new TestCaseData(new BoundingBox2D(), new BoundingBox2D(), true).SetName(
                        "Two empty bounding boxes have the same hash code.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(50, 50)),
                        new BoundingBox2D(new Point2(0, 0), new Size2(50, 50)), true).SetName(
                        "Two indentical non-empty bounding boxes have the same hash code.");
                yield return
                    new TestCaseData(new BoundingBox2D(new Point2(0, 0), new Size2(50, 50)),
                        new BoundingBox2D(new Point2(50, 50), new Size2(50, 50)), false).SetName(
                        "Two different non-empty bounding boxes do not have the same hash code.");
            }
        }

        [Test]
        [TestCaseSource(nameof(HashCodeTestCases))]
        public void HashCode(BoundingBox2D boundingBox1, BoundingBox2D boundingBox2, bool expectedThatHashCodesAreEqual)
        {
            var hashCode1 = boundingBox1.GetHashCode();
            var hashCode2 = boundingBox2.GetHashCode();
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
                    new TestCaseData(new BoundingBox2D(),
                        string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radius: {1}", new Point2(),
                            new Size2())).SetName(
                        "The empty segment has the expected string representation using the current culture.");
                yield return new TestCaseData(new BoundingBox2D(new Point2(5.1f, -5.123f), new Size2(5.4f, -5.4123f)),
                    string.Format(CultureInfo.CurrentCulture, "Centre: {0}, Radius: {1}", new Point2(5.1f, -5.123f),
                        new Size2(5.4f, -5.4123f))).SetName(
                    "A non-empty segment has the expected string representation using the current culture.");
            }
        }

        [Test]
        [TestCaseSource(nameof(StringCases))]
        public void String(BoundingBox2D boundingBox, string expectedString)
        {
            var actualString = boundingBox.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
