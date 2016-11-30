using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    public static class TestHelper
    {
        public static void AreEqual(Vector3 a, Vector3 b, double delta)
        {
            Assert.AreEqual(a.X, b.X, delta);
            Assert.AreEqual(a.Y, b.Y, delta);
            Assert.AreEqual(a.Z, b.Z, delta);
        }

        public static GraphicsDevice CreateGraphicsDevice()
        {
            return new GraphicsDevice(
                adapter: GraphicsAdapter.DefaultAdapter, 
                graphicsProfile: GraphicsProfile.HiDef, 
                presentationParameters: new PresentationParameters())
            {
                Viewport = new Viewport(0, 0, 800, 480)
            };
        }
    }
}