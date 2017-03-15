using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.ViewportAdapters
{
    [TestFixture]
    public class BoxingViewportAdapterTests
    {
        [Test]
        public void BoxingViewportAdapter_Letterbox_Test()
        {
            var gameWindow = new MockGameWindow();
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

            graphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);
            viewportAdapter.Reset();

            Assert.AreEqual(1024, graphicsDevice.Viewport.Width);
            Assert.AreEqual(614, graphicsDevice.Viewport.Height);
            Assert.AreEqual(BoxingMode.Letterbox, viewportAdapter.BoxingMode);
        }

        [Test]
        public void BoxingViewportAdapter_Pillarbox_Test()
        {
            var gameWindow = new MockGameWindow();
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

            graphicsDevice.Viewport = new Viewport(0, 0, 900, 500);
            viewportAdapter.Reset();

            Assert.AreEqual(833, graphicsDevice.Viewport.Width);
            Assert.AreEqual(500, graphicsDevice.Viewport.Height);
            Assert.AreEqual(BoxingMode.Pillarbox, viewportAdapter.BoxingMode);
        }

    }
}