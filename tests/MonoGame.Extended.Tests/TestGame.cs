using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public class TestGame : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        public TestGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            RunOneFrame();
        }

        protected override void Dispose(bool disposing)
        {
            ((IDisposable)_graphicsDeviceManager).Dispose();
            base.Dispose(disposing);
        }
    }
}