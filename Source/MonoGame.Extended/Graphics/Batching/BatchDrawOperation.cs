using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawOperation<TBatchItemData, TEffect>
        where TBatchItemData : struct, IBatchItemData<TBatchItemData, TEffect> where TEffect : Effect
    {
        internal readonly int StartVertex;
        internal int VertexCount;
        internal readonly int StartIndex;
        internal int IndexCount;
        internal byte PrimitiveType;
        internal TBatchItemData Data;

        internal BatchDrawOperation(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, TBatchItemData data)
        {
            PrimitiveType = (byte)primitiveType;
            StartVertex = startVertex;
            VertexCount = vertexCount;
            StartIndex = startIndex;
            IndexCount = indexCount;
            Data = data;
        }

        internal bool CanMergeIntoOperationOf(ref TBatchItemData otherData, byte primitiveType, int indexCount)
        {
            return Data.Equals(ref otherData) && IndexCount > 0 == indexCount > 0 && PrimitiveType == primitiveType && (PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip || PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip);
        }
    }
}
