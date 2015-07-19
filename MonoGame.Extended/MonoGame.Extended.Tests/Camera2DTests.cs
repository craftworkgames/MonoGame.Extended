using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class Camera2DTests
    {
        [Test]
        public void Camera2D_LookAt_Test()
        {
            var viewportAdapter = Substitute.For<ViewportAdapter>();
            viewportAdapter.VirtualWidth.Returns(800);
            viewportAdapter.VirtualHeight.Returns(480);
            var camera = new Camera2D(viewportAdapter);
            
            camera.LookAt(new Vector2(100, 200));

            Assert.AreEqual(new Vector2(-300, -40), camera.Position);
        }
    }
}