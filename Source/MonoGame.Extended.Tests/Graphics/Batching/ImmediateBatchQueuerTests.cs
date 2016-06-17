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
            var batchDrawer = new BatchDrawer<TestVertex>(graphicsDevice);
            var immediateBatchQueuer = new ImmediateBatchQueuer<TestVertex>(batchDrawer);
            Assert.IsNotNull(immediateBatchQueuer);
        }
    }
}
