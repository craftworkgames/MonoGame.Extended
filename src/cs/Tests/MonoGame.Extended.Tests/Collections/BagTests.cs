using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace MonoGame.Extended.Tests.Collections
{
    public class BagTests
    {
        [Fact]
        public void Bag_Enumeration_Does_Not_Allocate()
        {
            var bag = new Bag<int>();
            for (int i = 0; i < 100; i++) bag.Add(i);
            // ensure we have plenty of memory and that the heap only increases for the duration of this test
            Assert.True(GC.TryStartNoGCRegion(Unsafe.SizeOf<Bag<int>.BagEnumerator>() * 1000));
            var heapSize = GC.GetAllocatedBytesForCurrentThread();

            // this should NOT allocate
            foreach (int i in bag)
            {
                // assert methods cause the NoGCRegion to fail, so do this manually
                if (GC.GetAllocatedBytesForCurrentThread() != heapSize)
                    Assert.True(false);
            }

            // sanity check: this SHOULD allocate
            foreach (int _ in (IEnumerable<int>)bag)
            {
                // assert methods cause the NoGCRegion to fail, so do this manually
                if (GC.GetAllocatedBytesForCurrentThread() == heapSize)
                    Assert.True(false);
            }

            //  Wrap in if statement due to exception thrown when running test through
            //  cake build script or when debugging test script manually.
            if(GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
            {
                GC.EndNoGCRegion();
            }
        }

    }
}
