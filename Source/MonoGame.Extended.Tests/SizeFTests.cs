using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class SizeFTests
    {
        [Test]
        public void SizeF_ImplicitCastToVector2()
        {
            var sizeF = new SizeF(12.34f, 56.78f);
            Vector2 vector = sizeF;

            Assert.AreEqual(vector.X, sizeF.Width);
            Assert.AreEqual(vector.Y, sizeF.Height);
        }

        [Test]
        public void SizeF_ImplicitCastFromPoint()
        {
            var point = new Point(1, 2);
            SizeF sizeF = point;

            Assert.AreEqual(sizeF.Width, point.X);
            Assert.AreEqual(sizeF.Height, point.Y);
        }

        [Test]
        public void SizeF_ExplicitCastToSize()
        {
            var sizeF = new SizeF(12.34f, 56.78f);
            var size = (Size) sizeF;

            Assert.AreEqual(size.Width, (int)sizeF.Width);
            Assert.AreEqual(size.Height, (int)sizeF.Height);
        }

        [Test]
        public void SizeF_OperatorOverloads()
        {
            var size = new SizeF(10f, 5f);
            var halfSize = size / 2f;
            var doubleSize = size * 2f;

            Assert.AreEqual(halfSize, new SizeF(5f, 2.5f));
            Assert.AreEqual(doubleSize, new SizeF(20f, 10f));
        }
    }
}