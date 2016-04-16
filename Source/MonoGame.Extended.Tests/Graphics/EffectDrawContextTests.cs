using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Graphics
{
    [TestFixture]
    public class EffectDrawContextTests
    {
        private class TestDrawContext : IDrawContext
        {
            public uint SortKey
            {
                get { return 0; }
            }

            public int PassesCount
            {
                get { return 123; }
            }

            public void Begin()
            {
            }

            public void End()
            {
            }

            public void ApplyPass(int passIndex)
            {
            }
        }

        [TestCase]
        public void EffectDrawContext_Constructor()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            Assert.GreaterOrEqual(effectDrawContext.PassesCount, 1);
        }

        [TestCase]
        public void EffectDrawContext_ApplyPasses()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            var passesCount = effectDrawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                effectDrawContext.ApplyPass(passIndex);
            }
        }

        [TestCase]
        public void EffectDrawContext_EffectEquals()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            Assert.AreEqual(basicEffect, effectDrawContext.Effect);
        }

        [TestCase]
        public void EffectDrawContext_Equals()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(effectDrawContext.Equals(effectDrawContext));
            var effectDrawContext2 = new EffectDrawContext<BasicEffect>(basicEffect, 0);
            Assert.IsTrue(effectDrawContext.Equals(effectDrawContext2));
            var drawContext = new TestDrawContext();
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(effectDrawContext.Equals(drawContext));
        }
    }
}
