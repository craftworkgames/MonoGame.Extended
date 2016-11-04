﻿using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Graphics
{
    [TestFixture]
    public class PrimitiveTypeExtensionsTests
    {
        [Test]
        public void PrimitiveTypeExtensions_GetPrimitiveCount_LineStrip()
        {
            Assert.AreEqual(122, PrimitiveType.LineStrip.GetPrimitivesCount(123));
        }

        [Test]
        public void PrimitiveTypeExtensions_GetPrimitiveCount_LineList()
        {
            Assert.AreEqual(62, PrimitiveType.LineList.GetPrimitivesCount(124));
        }

        [Test]
        public void PrimitiveTypeExtensions_GetPrimitiveCount_TriangleStrip()
        {
            Assert.AreEqual(121, PrimitiveType.TriangleStrip.GetPrimitivesCount(123));
        }

        [Test]
        public void PrimitiveTypeExtensions_GetPrimitiveCount_Invalid()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ((PrimitiveType)7).GetPrimitivesCount(123);
            });
        }
    }
}
