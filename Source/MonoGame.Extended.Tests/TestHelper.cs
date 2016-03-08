using Microsoft.Xna.Framework;
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

        public static Game CreateGame()
        {
            var game = new Game();

            var graphicsDeviceManager = new GraphicsDeviceManager(game)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 400
            };
            graphicsDeviceManager.ApplyChanges();
            return game;
        }
    }
}