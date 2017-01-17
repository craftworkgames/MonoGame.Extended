using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Shapes
{
    [TestFixture]
    public class CircleTests
    {
        [Test]
        public void Circle_ConstructorsAndProperties()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);

            Assert.AreEqual(new CircleF() { Center = new Vector2(200.0f, 300.0f), Radius = 100.0f }, circle);
            Assert.AreEqual(200.0f - 100.0f, circle.Left);
            Assert.AreEqual(200.0f + 100.0f, circle.Right);
            Assert.AreEqual(300.0f - 100.0f, circle.Top);
            Assert.AreEqual(300.0f + 100.0f, circle.Bottom);
            Assert.AreEqual(new Point(200, 300), circle.Location);
            Assert.AreEqual(new Vector2(200.0f, 300.0f), circle.Center);
            Assert.AreEqual(100.0f, circle.Radius);
            Assert.AreEqual(false, circle.IsEmpty);
            Assert.AreEqual(true, new CircleF().IsEmpty);
            Assert.AreEqual(new CircleF(), CircleF.Empty);
        }

        [Test]
        public void Circle_ContainsPoint()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);

            var p1 = new Point(-1, -1);
            var p2 = new Point(110, 300);
            var p3 = new Point(200, 300);
            var p4 = new Point(290, 300);
            var p5 = new Point(400, 400);

            bool result;

            circle.Contains(ref p1, out result);
            Assert.AreEqual(false, result);
            circle.Contains(ref p2, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p3, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p4, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p5, out result);
            Assert.AreEqual(false, result);

            Assert.AreEqual(false, circle.Contains(p1));
            Assert.AreEqual(true, circle.Contains(p2));
            Assert.AreEqual(true, circle.Contains(p3));
            Assert.AreEqual(true, circle.Contains(p4));
            Assert.AreEqual(false, circle.Contains(p5));
        }

        [Test]
        public void Circle_ContainsVector2()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);

            var p1 = new Vector2(-1, -1);
            var p2 = new Vector2(110, 300);
            var p3 = new Vector2(200, 300);
            var p4 = new Vector2(290, 300);
            var p5 = new Vector2(400, 400);

            bool result;

            circle.Contains(ref p1, out result);
            Assert.AreEqual(false, result);
            circle.Contains(ref p2, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p3, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p4, out result);
            Assert.AreEqual(true, result);
            circle.Contains(ref p5, out result);
            Assert.AreEqual(false, result);

            Assert.AreEqual(false, circle.Contains(p1));
            Assert.AreEqual(true, circle.Contains(p2));
            Assert.AreEqual(true, circle.Contains(p3));
            Assert.AreEqual(true, circle.Contains(p4));
            Assert.AreEqual(false, circle.Contains(p5));
        }

        [Test]
        public void Circle_ContainsFloats()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);

            float x1 = -1; float y1 = -1;
            float x2 = 110; float y2 = 300;
            float x3 = 200; float y3 = 300;
            float x4 = 290; float y4 = 300;
            float x5 = 400; float y5 = 400;

            Assert.AreEqual(false, circle.Contains(x1, y1));
            Assert.AreEqual(true, circle.Contains(x2, y2));
            Assert.AreEqual(true, circle.Contains(x3, y3));
            Assert.AreEqual(true, circle.Contains(x4, y4));
            Assert.AreEqual(false, circle.Contains(x5, y5));
        }

        [Test]
        public void Circle_ContainsCircle()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);
            var circle1 = new CircleF(new Vector2(199.0f, 299.0f), 100.0f);
            var circle2 = new CircleF(new Vector2(200.0f, 300.0f), 25.0f);
            var circle3 = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);
            var circle4 = new CircleF(new Vector2(201.0f, 301.0f), 100.0f);

            bool result;

            circle.Contains(ref circle1, out result);
            Assert.AreEqual(false, result);

            circle.Contains(ref circle2, out result);
            Assert.AreEqual(true, result);

            circle.Contains(ref circle3, out result);
            Assert.AreEqual(true, result);

            circle.Contains(ref circle4, out result);
            Assert.AreEqual(false, result);

            Assert.AreEqual(false, circle.Contains(circle1));
            Assert.AreEqual(true, circle.Contains(circle2));
            Assert.AreEqual(true, circle.Contains(circle3));
            Assert.AreEqual(false, circle.Contains(circle4));
        }


        [Test]
        public void CircleF_Intersects_Rectangle_Test()
        {
            var circle = new CircleF(new Vector2(200, 200), 32);

            var rectangle1 = new Rectangle(210, 210, 64, 64);
            var rectangle2 = new Rectangle(130, 130, 64, 64);
            var rectangle3 = new Rectangle(230, 110, 64, 64);

            bool result;

            circle.Intersects(ref rectangle1, out result);
            Assert.IsTrue(result);

            circle.Intersects(ref rectangle2, out result);
            Assert.IsTrue(result);

            circle.Intersects(ref rectangle3, out result);
            Assert.IsFalse(result);

            Assert.IsTrue(circle.Intersects(rectangle1));
            Assert.IsTrue(circle.Intersects(rectangle2));
            Assert.IsFalse(circle.Intersects(rectangle3));
        }

        [Test]
        public void CircleF_Intersects_CircleF_Test()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);

            var circle1 = new CircleF(new Vector2(350.0f, 300.0f), 100.0f);
            var circle2 = new CircleF(new Vector2(400.0f, 300.0f), 100.0f);

            bool result;

            circle.Intersects(ref circle1, out result);
            Assert.IsTrue(result);

            circle.Intersects(ref circle2, out result);
            Assert.IsFalse(result);

            Assert.IsTrue(circle.Intersects(circle1));
            Assert.IsFalse(circle.Intersects(circle2));
        }

        [Test]
        public void Circle_Inflate()
        {
            var circle = new CircleF(new Vector2(200.0f, 300.0f), 100.0f);
            circle.Inflate(100.0f);
            Assert.AreEqual(new CircleF(new Vector2(100.0f, 200.0f), 300.0f), circle);
        }

        [Test]
        public void Circle_ToStringTest()
        {
            Assert.AreEqual("{Center:{X:200 Y:300} Radius:100}", new CircleF(new Vector2(200.0f, 300.0f), 100.0f).ToString());
        }

        [Test]
        public void Circle_ToRectangleTest()
        {
            var actual = new CircleF(center: new Vector2(200, 300), radius: 100).ToRectangle();
            var expected = new Rectangle(100, 200, 200, 200);
            Assert.AreEqual(expected, actual);
        }
    }
}
