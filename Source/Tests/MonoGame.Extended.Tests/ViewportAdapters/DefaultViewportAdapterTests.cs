//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.ViewportAdapters;
//using Xunit;

//namespace MonoGame.Extended.Tests.ViewportAdapters
//{
//    
//    public class DefaultViewportAdapterTests
//    {
//        [Fact]
//        public void DefaultViewportAdapter_Test()
//        {
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var viewportAdapter = new DefaultViewportAdapter(graphicsDevice);

//            graphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);

//            Assert.Equal(1024, viewportAdapter.ViewportWidth);
//            Assert.Equal(768, viewportAdapter.ViewportHeight);
//            Assert.Equal(viewportAdapter.ViewportWidth, viewportAdapter.VirtualWidth);
//            Assert.Equal(viewportAdapter.ViewportHeight, viewportAdapter.VirtualHeight);
//            Assert.Equal(Matrix.Identity, viewportAdapter.GetScaleMatrix());
//        }
//    }
//}