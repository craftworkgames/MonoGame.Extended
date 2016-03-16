using System.Threading;

namespace MonoGame.Extended.Collections
{
    public partial class ObjectPool
    {
        public class Diagnostics
        {
            private int _instancesInUseCount;
            private int _instancesResetFailedCount;
            private int _instancesRessurectedCount;
            private int _instancesHitCount;
            private int _instancesMissedCount;
            private int _instancesCreatedCount;
            private int _instancesDestroyedCount;
            private int _instancesOverflowCount;
            private int _instancesReturnedCount;

            public int InstancesInUseCount
            {
                get { return _instancesInUseCount; }
            }

            public int InstancesResetFailedCount
            {
                get { return _instancesResetFailedCount; }
            }

            public int InstancesRessurectedCount
            {
                get { return _instancesRessurectedCount; }
            }

            public int InstancesHitCount
            {
                get { return _instancesHitCount; }
            }

            public int InstancesMissedCount
            {
                get { return _instancesMissedCount; }
            }

            public int InstancesCreatedCount
            {
                get { return _instancesCreatedCount; }
            }

            public int InstancesDestroyedCount
            {
                get { return _instancesDestroyedCount; }
            }

            public int InstancesOverflowCount
            {
                get { return _instancesOverflowCount; }
            }

            public int InstancesReturnedCount
            {
                get { return _instancesReturnedCount; }
            }

            internal void IncrementInstancesCreated()
            {
                Interlocked.Increment(ref _instancesCreatedCount);
            }

            internal void IncrementInstancesDestroyed()
            {
                Interlocked.Increment(ref _instancesDestroyedCount);
            }

            internal void IncrementInstancesInUse()
            {
                Interlocked.Increment(ref _instancesInUseCount);
            }

            internal void DecrementInstancesInUse()
            {
                Interlocked.Decrement(ref _instancesInUseCount);
            }

            internal void IncrementInstancesHit()
            {
                Interlocked.Increment(ref _instancesHitCount);
            }

            internal void IncrementInstancesMissed()
            {
                Interlocked.Increment(ref _instancesMissedCount);
            }

            internal void IncrementInstancesOverflow()
            {
                Interlocked.Increment(ref _instancesOverflowCount);
            }

            internal void IncrementInstancesResetStateFailed()
            {
                Interlocked.Increment(ref _instancesResetFailedCount);
            }

            internal void IncrementInstancesRessurected()
            {
                Interlocked.Increment(ref _instancesRessurectedCount);
            }

            internal void IncrementInstancesReturned()
            {
                Interlocked.Increment(ref _instancesReturnedCount);
            }
        }
    }
}
