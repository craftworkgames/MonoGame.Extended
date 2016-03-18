using System.Collections.Generic;
using MonoGame.Extended.Collections;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Collections
{
    [TestFixture]
    public class ObjectPoolTests
    {
        private class TestPoolable : IPoolable
        {
            public bool ResetState()
            {
                return true;
            }

            public void Dispose()
            {
            }
        }

        [Test]
        public void ObjectPool_UseTest()
        {
            var pool = new ObjectPool<TestPoolable>(5, 10, () => new TestPoolable());

            var objects = new Stack<TestPoolable>();
            for (var i = 0; i < 5; i++)
            {
                var obj = pool.GetObject();
                if (obj != null)
                {
                    objects.Push(obj);
                }

                Assert.AreEqual(5 - i - 1, pool.AvailableCount);
                Assert.AreEqual(objects.Count, pool.InUseCount);
                Assert.AreEqual(5, pool.CreatedCount);
                Assert.AreEqual(0, pool.DestroyedCount);
                Assert.AreEqual(i + 1, pool.HitCount);
                Assert.AreEqual(0, pool.MissedCount);
                Assert.AreEqual(0, pool.ReturnedCount);
                Assert.AreEqual(0, pool.OverflowCount);
                Assert.AreEqual(0, pool.ResetFailedCount);
                Assert.AreEqual(0, pool.RessurectedCount);
            }
            // 5 objects

            for (var i = 0; i < 10; i++)
            {
                var obj = pool.GetObject();
                if (obj != null)
                {
                    objects.Push(obj);
                }

                Assert.AreEqual(0, pool.AvailableCount);
                Assert.AreEqual(objects.Count, pool.InUseCount);
                Assert.AreEqual(5 + i + 1, pool.CreatedCount);
                Assert.AreEqual(0, pool.DestroyedCount);
                Assert.AreEqual(5, pool.HitCount);
                Assert.AreEqual(i + 1, pool.MissedCount);
                Assert.AreEqual(0, pool.ReturnedCount);
                Assert.AreEqual(0, pool.OverflowCount);
                Assert.AreEqual(0, pool.ResetFailedCount);
                Assert.AreEqual(0, pool.RessurectedCount);
            }
            // 15 objects

            for (var i = 0; i < 5; i++)
            {
                var obj = objects.Pop();
                pool.ReturnObject(obj);

                Assert.AreEqual(i + 1, pool.AvailableCount);
                Assert.AreEqual(objects.Count, pool.InUseCount);
                Assert.AreEqual(15, pool.CreatedCount);
                Assert.AreEqual(0, pool.DestroyedCount);
                Assert.AreEqual(5, pool.HitCount);
                Assert.AreEqual(10, pool.MissedCount);
                Assert.AreEqual(i + 1, pool.ReturnedCount);
                Assert.AreEqual(0, pool.OverflowCount);
                Assert.AreEqual(0, pool.ResetFailedCount);
                Assert.AreEqual(0, pool.RessurectedCount);
            }
        }
    }
}
