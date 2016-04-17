using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using NUnit.Framework;
using TestVertex = Microsoft.Xna.Framework.Graphics.VertexPositionColor;

namespace MonoGame.Extended.Tests.Graphics.Batching
{
    [TestFixture]
    public class ImmediateBatchQueuerTests
    {
        [Test]
        public void ImmediateBatchQueuer_Constructor()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            var userPrimitivesBatcher = new UserPrimitivesBatchDrawer<TestVertex>(graphicsDevice);
            var immediateBatchQueuer = new ImmediateBatchQueuer<TestVertex>(userPrimitivesBatcher);
            Assert.IsNotNull(immediateBatchQueuer);
        }
    }
}
