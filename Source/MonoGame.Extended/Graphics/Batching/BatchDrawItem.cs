using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawItem<TBatchItemData>
        where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal TBatchItemData Data;
        internal readonly int StartIndex;
        internal int PrimitiveCount;
        internal byte PrimitiveType;

        internal BatchDrawItem(PrimitiveType primitiveType, int startIndex, int primitiveCount, TBatchItemData data)
        {
            PrimitiveType = (byte)primitiveType;
            StartIndex = startIndex;
            PrimitiveCount = primitiveCount;
            Data = data;
        }

        internal bool CanMergeIntoItem(ref TBatchItemData otherData, byte primitiveType)
        {
            return Data.Equals(ref otherData) && PrimitiveType == primitiveType && (PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip || PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip);
        }
    }
}
