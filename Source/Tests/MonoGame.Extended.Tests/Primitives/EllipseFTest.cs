﻿//using Microsoft.Xna.Framework;
//using Xunit;

//namespace MonoGame.Extended.Tests.Primitives
//{
//    
//    public class EllipseFTest
//    {
//        [Fact]
//        [TestCase(-1, -1, Result = false)]
//        [TestCase(110, 300, Result = true)]
//        [TestCase(200, 300, Result = true)]
//        [TestCase(290, 300, Result = true)]
//        [TestCase(400, 400, Result = false)]
//        public bool ContainsPoint_Circle(int x, int y)
//        {
//            var ellipse = new EllipseF(new Vector2(200.0f, 300.0f), 100.0f, 100.0f);

//            return ellipse.Contains(x, y);
//        }

//        [Fact]
//        [TestCase(299, 400, Result = false)]
//        [TestCase(501, 400, Result = false)]
//        [TestCase(400, 199, Result = false)]
//        [TestCase(400, 601, Result = false)]
//        [TestCase(301, 400, Result = true)]
//        [TestCase(499, 400, Result = true)]
//        [TestCase(400, 201, Result = true)]
//        [TestCase(400, 599, Result = true)]
//        public bool ContainsPoint_NonCircle(int x, int y)
//        {
//            var ellipse = new EllipseF(new Vector2(400.0f, 400.0f), 100.0f, 200.0f);

//            return ellipse.Contains(x, y);
//        }
//    }
//}