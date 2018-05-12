//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Xunit;

//namespace MonoGame.Extended.Tests
//{
//    public static class TestHelper
//    {
//        public static void AreEqual(Vector3 a, Vector3 b, double delta)
//        {
//            Assert.Equal(a.X, b.X, delta);
//            Assert.Equal(a.Y, b.Y, delta);
//            Assert.Equal(a.Z, b.Z, delta);
//        }

//        public static GraphicsDevice CreateGraphicsDevice()
//        {
//            return new GraphicsDevice(
//                GraphicsAdapter.DefaultAdapter,
//                GraphicsProfile.HiDef,
//                new PresentationParameters())
//            {
//                Viewport = new Viewport(0, 0, 800, 480)
//            };
//        }
//    }
//}