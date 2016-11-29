using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawCommand<TCommandData> : IComparable<BatchDrawCommand<TCommandData>>
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal float SortKey;
        internal int StartIndex;
        internal int PrimitiveCount;
        internal PrimitiveType PrimitiveType;
        internal TCommandData Data;

        internal BatchDrawCommand(PrimitiveType primitiveType, int startIndex, int primitiveCount, float sortKey, TCommandData data)
        {
            PrimitiveType = primitiveType;
            SortKey = sortKey;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Data = data;
        }

        internal bool CanMergeWith(PrimitiveType primitiveType, float sortKey, ref TCommandData commandData)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return PrimitiveType == primitiveType && SortKey == sortKey && Data.Equals(ref commandData);
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