using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Shapes
{
    [TestFixture]
    public class RectangleFTests
    {
        [Test]
        public void ConstructorsAndProperties()
        {
            // Create a RectangleF from ints and from floats.
            var intRectangleF = new RectangleF(10, 20, 64, 64);
            var floatRectangleF = new RectangleF(7.5f, 17.5f, 62.5f, 62.5f);

            // Test basic constructing
            Assert.AreEqual(new RectangleF(10, 20, 64, 64), intRectangleF);
            Assert.AreEqual(new RectangleF(10f, 20f, 64f, 64f), intRectangleF);

            Assert.AreNotEqual(new RectangleF(10f, 20f, 64f, 64f), floatRectangleF);
            Assert.AreNotEqual(new RectangleF(7.5f, 17.5f, 62.5f, 62.5f), intRectangleF);

            Assert.AreEqual(new RectangleF(7.5f, 17.5f, 62.5f, 62.5f), floatRectangleF);

            // Test int + float during construction
            Assert.AreEqual(new RectangleF(5 + 2.5f, 10 + 7.5f, 60 + 2.5f, 60 + 2.5f), floatRectangleF);

            // Test float + float during construction
            Assert.AreEqual(new RectangleF(3.75f + 3.75f, 17.5f, 62.5f, 62.5f), floatRectangleF);

            // Test sides
            Assert.AreEqual(7.5f, floatRectangleF.Left);
            Assert.AreEqual(7.5f + 62.5f, floatRectangleF.Right);
            Assert.AreEqual(17.5f, floatRectangleF.Top);
            Assert.AreEqual(17.5f + 62.5f, floatRectangleF.Bottom);

            // IsEmpty and static Empty
            Assert.AreEqual(false, floatRectangleF.IsEmpty);
            Assert.AreEqual(false, intRectangleF.IsEmpty);
            Assert.AreEqual(true, new RectangleF().IsEmpty);
            Assert.AreEqual(new RectangleF(), RectangleF.Empty);
        }

        #region Deprecated Tests

        [Test]
        public void RectangleF_Touching_Test()
        {
            var rect1 = new RectangleF(0, 0, 32, 32);
            var rect2 = new RectangleF(32, 32, 32, 32);

            Assert.IsFalse(rect1.Intersects(rect2));
        }

        [Test]
        public void Rectangle_Touching_Test()
        {
            var rect1 = new Rectangle(0, 0, 32, 32);
            var rect2 = new Rectangle(32, 32, 32, 32);

            Assert.IsFalse(rect1.Intersects(rect2));
        }

        [Test]
        public void RectangleF_Intersects_Test()
        {
            var rect1 = new RectangleF(0.1f, 0.1f, 32, 32);
            var rect2 = new RectangleF(32, 32, 32, 32);

            Assert.IsTrue(rect1.Intersects(rect2));
        }

        [Test]
        public void Rectangle_Intersects_Test()
        {
            var rect1 = new Rectangle(1, 1, 32, 32);
            var rect2 = new Rectangle(32, 32, 32, 32);

            Assert.IsTrue(rect1.Intersects(rect2));
        }

        #endregion

    }
}