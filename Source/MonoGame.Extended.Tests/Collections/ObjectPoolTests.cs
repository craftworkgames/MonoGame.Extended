using System.Collections.Generic;
using MonoGame.Extended.Collections;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Collections
{
    [TestFixture]
    public class ObjectPoolTests
    {
        private class TestPoolable : Poolable
        {
        }

        [Test]
        public void ObjectPool_UseTest()
        {
            var pool = new ObjectPool<Poolable>(5, 10, () => new TestPoolable());

            var objects = new Stack<Poolable>();
            for (var i = 0; i < 5; i++)
            {
                var obj = pool.GetObject();
                if (obj != null)
                {
                    objects.Push(obj);
                }

                Assert.AreEqual(5 - i - 1, pool.AvailableCount);
                Assert.AreEqual(objects.Count, pool.Diagnostics.InstancesInUseCount);
                Assert.AreEqual(5, pool.Diagnostics.InstancesCreatedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesDestroyedCount);
                Assert.AreEqual(i + 1, pool.Diagnostics.InstancesHitCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesMissedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesReturnedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesOverflowCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesResetFailedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesRessurectedCount);
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
                Assert.AreEqual(objects.Count, pool.Diagnostics.InstancesInUseCount);
                Assert.AreEqual(5 + i + 1, pool.Diagnostics.InstancesCreatedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesDestroyedCount);
                Assert.AreEqual(5, pool.Diagnostics.InstancesHitCount);
                Assert.AreEqual(i + 1, pool.Diagnostics.InstancesMissedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesReturnedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesOverflowCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesResetFailedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesRessurectedCount);
            }
            // 15 objects

            for (var i = 0; i < 5; i++)
            {
                var obj = objects.Pop();
                pool.ReturnObject(obj);

                Assert.AreEqual(i + 1, pool.AvailableCount);
                Assert.AreEqual(objects.Count, pool.Diagnostics.InstancesInUseCount);
                Assert.AreEqual(15, pool.Diagnostics.InstancesCreatedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesDestroyedCount);
                Assert.AreEqual(5, pool.Diagnostics.InstancesHitCount);
                Assert.AreEqual(10, pool.Diagnostics.InstancesMissedCount);
                Assert.AreEqual(i + 1, pool.Diagnostics.InstancesReturnedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesOverflowCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesResetFailedCount);
                Assert.AreEqual(0, pool.Diagnostics.InstancesRessurectedCount);
            }
        }
    }
}
