using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.ViewportAdapters
{
    [TestFixture]
    public class DefaultViewportAdapterTests
    {
        [Test]
        public void DefaultViewportAdapter_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var viewportAdapter = new DefaultViewportAdapter(graphicsDevice);

            graphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);

            Assert.AreEqual(1024, viewportAdapter.ViewportWidth);
            Assert.AreEqual(768, viewportAdapter.ViewportHeight);
            Assert.AreEqual(viewportAdapter.ViewportWidth, viewportAdapter.VirtualWidth);
            Assert.AreEqual(viewportAdapter.ViewportHeight, viewportAdapter.VirtualHeight);
            Assert.AreEqual(Matrix.Identity, viewportAdapter.GetScaleMatrix());
        }
    }
}