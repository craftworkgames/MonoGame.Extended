//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.ViewportAdapters;
//using Xunit;

//namespace MonoGame.Extended.Tests.ViewportAdapters
//{
//    
//    public class BoxingViewportAdapterTests
//    {
//        [Fact]
//        public void BoxingViewportAdapter_Letterbox_Test()
//        {
//            var gameWindow = new MockGameWindow();
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

//            graphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);
//            viewportAdapter.Reset();

//            Assert.Equal(1024, graphicsDevice.Viewport.Width);
//            Assert.Equal(614, graphicsDevice.Viewport.Height);
//            Assert.Equal(BoxingMode.Letterbox, viewportAdapter.BoxingMode);
//        }

//        [Fact]
//        public void BoxingViewportAdapter_Pillarbox_Test()
//        {
//            var gameWindow = new MockGameWindow();
//            var graphicsDevice = TestHelper.CreateGraphicsDevice();
//            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, 800, 480);

//            graphicsDevice.Viewport = new Viewport(0, 0, 900, 500);
//            viewportAdapter.Reset();

//            Assert.Equal(833, graphicsDevice.Viewport.Width);
//            Assert.Equal(500, graphicsDevice.Viewport.Height);
//            Assert.Equal(BoxingMode.Pillarbox, viewportAdapter.BoxingMode);
//        }

//    }
//}