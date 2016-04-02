using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using NUnit.Framework;
using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace MonoGame.Extended.Tests.Graphics.Batching
{
    [TestFixture]
    public class UserPrimitivesBatcherTests
    {
        [Test]
        public void UserPrimitivesBatcher_Constructor()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            Assert.IsNotNull(userPrimitivesBatcher);
        }

        [Test]
        public void UserPrimitivesBatcher_MaximumBatchSize()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            Assert.Greater(userPrimitivesBatcher.MaxiumumBatchSize, 4);
        }

        [Test]
        public void UserPrimitivesBatcher_SelectVertices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            var vertices = new TestVertex[9];
            userPrimitivesBatcher.Select(vertices);
        }

        [Test]
        public void UserPrimitivesBatcher_SelectVerticesWithIndices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            var vertices = new TestVertex[9];
            var indices = new short[9];
            userPrimitivesBatcher.Select(vertices, indices);
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVertices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            var vertices = new TestVertex[9];
            var indices = new short[9];
            userPrimitivesBatcher.Select(vertices);
            userPrimitivesBatcher.Draw(PrimitiveType.TriangleList, 0, 9, null);
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVerticesWithIndices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice, defaultDrawContext);
            var vertices = new TestVertex[9];
            var indices = new short[9];
            userPrimitivesBatcher.Select(vertices, indices);
            userPrimitivesBatcher.Draw(PrimitiveType.TriangleList, 0, 9, 0, 9, null);
        }
    }
}
