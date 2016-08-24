using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchItem<TDrawContext> : IComparable<BatchItem<TDrawContext>>
        where TDrawContext : struct, IDrawContext<TDrawContext>
    {
        internal TDrawContext Context;
        internal readonly uint SortKey;
        internal readonly int StartIndex;
        internal int PrimitiveCount;

        internal BatchItem(uint sortKey, int startIndex, int primitiveCount, TDrawContext context)
        {
            SortKey = sortKey;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Context = context;
        }

        internal bool CanMergeWith(uint sortKey, ref TDrawContext otherContext)
        {
            return SortKey == sortKey && Context.Equals(ref otherContext);
        }

        public int CompareTo(BatchItem<TDrawContext> other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return SortKey.CompareTo(other.SortKey);
        }
    }
}
