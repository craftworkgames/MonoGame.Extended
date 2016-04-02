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
            public int PassesCount
            {
                get { return 123; }
            }

            public void ApplyPass(int passIndex)
            {
            }
        }

        private class TestEffectDrawContext : EffectDrawContext<BasicEffect>
        {
            public TestEffectDrawContext(BasicEffect effect)
                : base(effect)
            {
            }
        }

        [TestCase]
        public void EffectDrawContext_Constructor()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            Assert.GreaterOrEqual(effectDrawContext.PassesCount, 1);
        }

        [TestCase]
        public void EffectDrawContext_ApplyPasses()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
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
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            Assert.AreEqual(basicEffect, effectDrawContext.Effect);
        }

        [TestCase]
        public void EffectDrawContext_Equals()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var basicEffect = new BasicEffect(graphicsDevice);
            var effectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(effectDrawContext.Equals(effectDrawContext));
            var effectDrawContext2 = new EffectDrawContext<BasicEffect>(basicEffect);
            Assert.IsTrue(effectDrawContext.Equals(effectDrawContext2));
            var drawContext = new TestDrawContext();
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(effectDrawContext.Equals(drawContext));
        }
    }
}
