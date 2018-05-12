//using Microsoft.Xna.Framework;
//using Xunit;

//namespace MonoGame.Extended.Tests.Primitives
//{
//    
//    public class RectangleFTests
//    {
//        [Fact]
//        public void RectangleF_Intersects_Test()
//        {
//            var rect1 = new RectangleF(0, 0, 32, 32);
//            var rect2 = new RectangleF(32, 32, 32, 32);

//            Assert.IsFalse(rect1.Intersects(rect2));
//        }

//        [Fact]
//        public void Rectangle_Intersects_Test()
//        {
//            var rect1 = new Rectangle(0, 0, 32, 32);
//            var rect2 = new Rectangle(32, 32, 32, 32);

//            Assert.IsFalse(rect1.Intersects(rect2));
//        }

//        [Fact]
//        public void PassVector2AsConstructorParameter_Test()
//        {
//            var rect1 = new RectangleF(new Vector2(0, 0), new Size2(12.34f, 56.78f));
//            var rect2 = new RectangleF(new Vector2(0, 0), new Vector2(12.34f, 56.78f));

//            Assert.Equal(rect1, rect2);
//        }

//        [Fact]
//        public void PassPointAsConstructorParameter_Test()
//        {
//            var rect1 = new RectangleF(new Vector2(0, 0), new Size2(12, 56));
//            var rect2 = new RectangleF(new Vector2(0, 0), new Size2(12, 56));

//            Assert.Equal(rect1, rect2);
//        }
//    }
//}