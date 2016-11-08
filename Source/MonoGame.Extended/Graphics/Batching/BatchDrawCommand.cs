using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawCommand<TCommandData> : IComparable<BatchDrawCommand<TCommandData>>
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal float SortKey;
        internal ushort StartIndex;
        internal ushort PrimitiveCount;
        internal TCommandData Data;

        internal BatchDrawCommand(ushort startIndex, ushort primitiveCount, float sortKey, TCommandData data)
        {
            SortKey = sortKey;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Data = data;
        }

        internal bool CanMergeWith(float sortKey, ref TCommandData commandData)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return SortKey == sortKey && Data.Equals(ref commandData);
        }

        public int CompareTo(BatchDrawCommand<TCommandData> other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
           var result = SortKey.CompareTo(other.SortKey);
            if (result == 0)
                result = Data.CompareTo(other.Data);
           if (result == 0)
                result = StartIndex.CompareTo(other.StartIndex);
            return result;
        }
    }
}