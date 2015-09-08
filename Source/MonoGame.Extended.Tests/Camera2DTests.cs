using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
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

        [Test]
        public void Camera2D_GetBoundingFrustum_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var camera = new Camera2D(graphicsDevice);
            var boundingFrustum = camera.GetBoundingFrustum();
            var corners = boundingFrustum.GetCorners();

            const float delta = 0.01f;
            TestHelper.AreEqual(new Vector3(0,   0,   1), corners[0], delta);
            TestHelper.AreEqual(new Vector3(800, 0,   1), corners[1], delta);
            TestHelper.AreEqual(new Vector3(800, 480, 1), corners[2], delta);
            TestHelper.AreEqual(new Vector3(0,   480, 1), corners[3], delta);
            TestHelper.AreEqual(new Vector3(0,   0,   0), corners[4], delta);
            TestHelper.AreEqual(new Vector3(800, 0,   0), corners[5], delta);
            TestHelper.AreEqual(new Vector3(800, 480, 0), corners[6], delta);
            TestHelper.AreEqual(new Vector3(0,   480, 0), corners[7], delta);
        }

        [Test]
        public void Camera2D_ContainsPoint_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var camera = new Camera2D(graphicsDevice);

            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Point(1, 1)));
            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Point(799, 479)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Point(-1, -1)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Point(801, 481)));
        }

        [Test]
        public void Camera2D_ContainsVector2_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var camera = new Camera2D(graphicsDevice);

            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Vector2(799.5f, 479.5f)));
            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Vector2(0.5f, 0.5f)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Vector2(-0.5f, -0.5f)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Vector2(800.5f, 480.5f)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Vector2(-0.5f, 240f)));
            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Vector2(0.5f, 240f)));
            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Vector2(799.5f, 240f)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Vector2(800.5f, 240f)));
        }

        [Test]
        public void Camera2D_ContainsRectangle_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var camera = new Camera2D(graphicsDevice);

            Assert.AreEqual(ContainmentType.Intersects, camera.Contains(new Rectangle(-50, -50, 100, 100)));
            Assert.AreEqual(ContainmentType.Contains, camera.Contains(new Rectangle(50, 50, 100, 100)));
            Assert.AreEqual(ContainmentType.Disjoint, camera.Contains(new Rectangle(850, 500, 100, 100)));
        }
    }
}