using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchCommand<TCommandContext> : IComparable<BatchCommand<TCommandContext>>
        where TCommandContext : struct, IBatchCommandContext
    {
        internal readonly uint SortKey;
        internal TCommandContext Context;
        internal readonly ushort StartIndex;
        internal readonly ushort PrimitiveCount;

        internal BatchCommand(uint sortKey, TCommandContext context, ushort startIndex, ushort primitiveCount)
        {
            SortKey = sortKey;
            Context = context;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
        }

        public int CompareTo(BatchCommand<TCommandContext> other)
        {
            return -SortKey.CompareTo(other.SortKey);
        }
    }
}
