using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Tests.ViewportAdapters
{
    [TestFixture]
    public class BoxingViewportAdapterTests
    {
        private Game _game;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            _game = TestHelper.CreateGame();
            Task.Run(() =>
            {
                _game.Run();
            });
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _game.Dispose();
        }

        [Test]
        public void BoxingViewportAdapter_Letterbox_Test()
        {
            var gameWindow = _game.Window;
            var graphicsDevice = _game.GraphicsDevice;
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

            graphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);

            Assert.AreEqual(1024, graphicsDevice.Viewport.Width);
            Assert.AreEqual(614, graphicsDevice.Viewport.Height);
            Assert.AreEqual(BoxingMode.Letterbox, viewportAdapter.BoxingMode);
        }

        [Test]
        public void BoxingViewportAdapter_Pillarbox_Test()
        {
            var gameWindow = _game.Window;
            var graphicsDevice = _game.GraphicsDevice;
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

            graphicsDevice.Viewport = new Viewport(0, 0, 900, 500);

            Assert.AreEqual(833, graphicsDevice.Viewport.Width);
            Assert.AreEqual(500, graphicsDevice.Viewport.Height);
            Assert.AreEqual(BoxingMode.Pillarbox, viewportAdapter.BoxingMode);
        }
    }

}

