using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Primitives
{
    [TestFixture]
    public class RectangleFTests
    {
        [Test]
        public void RectangleF_Intersects_Test()
        {
            var rect1 = new RectangleF(0, 0, 32, 32);
            var rect2 = new RectangleF(32, 32, 32, 32);

            Assert.IsFalse(rect1.Intersects(rect2));
        }

        [Test]
        public void Rectangle_Intersects_Test()
        {
            var rect1 = new Rectangle(0, 0, 32, 32);
            var rect2 = new Rectangle(32, 32, 32, 32);

            Assert.IsFalse(rect1.Intersects(rect2));
        }

        [Test]
        public void PassVector2AsConstructorParameter_Test()
        {
            var rect1 = new RectangleF(new Vector2(0, 0), new Size2(12.34f, 56.78f));
            var rect2 = new RectangleF(new Vector2(0, 0), new Vector2(12.34f, 56.78f));

            Assert.AreEqual(rect1, rect2);
        }

        [Test]
        public void PassPointAsConstructorParameter_Test()
        {
            var rect1 = new RectangleF(new Vector2(0, 0), new Size2(12, 56));
            var rect2 = new RectangleF(new Vector2(0, 0), new Size2(12, 56));

            Assert.AreEqual(rect1, rect2);
        }
    }
}