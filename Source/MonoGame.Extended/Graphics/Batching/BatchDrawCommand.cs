using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawCommand<TCommandData> : IComparable<BatchDrawCommand<TCommandData>>
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal readonly uint SortKey;
        internal ushort StartIndex;
        internal ushort PrimitiveCount;
        internal TCommandData Data;

        internal BatchDrawCommand(ushort startIndex, ushort primitiveCount, uint sortKey, TCommandData data)
        {
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            SortKey = sortKey;
            Data = data;
        }

        internal bool CanMergeWith(uint sortKey, ref TCommandData commandData)
        {
            return (SortKey == sortKey) && Data.Equals(ref commandData);
        }

        public int CompareTo(BatchDrawCommand<TCommandData> other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return SortKey.CompareTo(other.SortKey);
        }
    }
}