using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Collections
{

    [TestFixture]
    public class PoolTests
    {
        private class TestPoolable : IPoolable
        {
            private ReturnToPoolDelegate _returnFunction;

            public int Value { get; }
            public int ResetValue { get; private set; }

            public TestPoolable(int value)
            {
                Value = value;
                ResetValue = -1;
            }

            void IPoolable.Initialize(ReturnToPoolDelegate returnFunction)
            {
                _returnFunction = returnFunction;
            }

            public void Return()
            {
                if (_returnFunction == null)
                {
                    return;
                }
                Reset();
                _returnFunction.Invoke(this);
                _returnFunction = null;
            }

            private void Reset()
            {
                ResetValue = 0;
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Pool_Constructor_Default(int count)
        {
            var pool = new Pool<TestPoolable>(count, i => new TestPoolable(i));
            Assert.IsTrue(pool.Count == 0);
            foreach (var poolable in pool)
            {
                Assert.Fail();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Pool_Create(int count)
        {
            var pool = new Pool<TestPoolable>(count, i => new TestPoolable(i));

            for (var i = 0; i < count; i++)
            {
                var poolable = pool.Request();
                Assert.IsTrue(poolable.Value == i);
            }

            var nextPoolable = pool.Request();
            Assert.IsNull(nextPoolable);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Pool_ForEach_Iteration(int count)
        {
            var pool = new Pool<TestPoolable>(count, i => new TestPoolable(i));

            for (var i = 0; i < count; i++)
            {
                var poolable = pool.Request();
                Assert.IsTrue(poolable.Value == i);
            }

            var counter = 0;
            foreach (var poolable in pool)
            {
                Assert.IsTrue(poolable.Value == counter);
                counter++;
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Pool_Return(int count)
        {
            var pool = new Pool<TestPoolable>(count, i => new TestPoolable(i));

            for (var i = 0; i < count; i++)
            {
                var poolable = pool.Request();
                Assert.IsTrue(poolable.Value == i);
            }

            var counter = count;
            foreach (var poolable in pool)
            {
                poolable.Return();
                Assert.IsTrue(poolable.ResetValue == 0);
                counter--;
                Assert.IsTrue(pool.Count == counter);
            }
        }
    }
}
