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
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            Assert.IsNotNull(userPrimitivesBatcher);
        }

        [Test]
        public void UserPrimitivesBatcher_MaximumBatchSize()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            Assert.Greater(userPrimitivesBatcher.MaximumBatchSize, 4);
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVertices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var drawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            var vertices = new TestVertex[9];
            userPrimitivesBatcher.Begin(drawContext, vertices);
            userPrimitivesBatcher.Draw(PrimitiveType.TriangleList, 0, 9);
            userPrimitivesBatcher.End();
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVerticesWithIndices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var drawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            var vertices = new TestVertex[9];
            var indices = new short[9];
            userPrimitivesBatcher.Begin(drawContext, vertices, indices);
            userPrimitivesBatcher.Draw(PrimitiveType.TriangleList, 0, 9, 0, 9);
            userPrimitivesBatcher.End();
        }
    }
}
