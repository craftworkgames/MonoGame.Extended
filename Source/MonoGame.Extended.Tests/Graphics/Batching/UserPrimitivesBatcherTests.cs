using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using NUnit.Framework;
using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace MonoGame.Extended.Tests.Graphics.Batching
{
    internal class BasicEffectMaterial : EffectMaterial<BasicEffect>
    {
        public BasicEffectMaterial(BasicEffect effect)
            : base(effect)
        {
        }

        public override void Apply(out Effect effect)
        {
            effect = Effect;
        }
    }

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
            Assert.Greater(userPrimitivesBatcher.MaximumVerticesCount, 4);
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVertices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            var vertices = new TestVertex[9];
            userPrimitivesBatcher.Select(vertices);
            var material = new BasicEffectMaterial(new BasicEffect(graphicsDevice));
            userPrimitivesBatcher.Draw(material, PrimitiveType.TriangleList, 0, 9);
        }

        [Test]
        public void UserPrimitivesBatcher_DrawVerticesWithIndices()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            var vertices = new TestVertex[9];
            var indices = new short[9];
            userPrimitivesBatcher.Select(vertices, indices);
            var material = new BasicEffectMaterial(new BasicEffect(graphicsDevice));
            userPrimitivesBatcher.Draw(material, PrimitiveType.TriangleList, 0, 9, 0, 9);
        }
    }
}
