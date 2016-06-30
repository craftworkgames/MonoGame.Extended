using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawOperation<TBatchItemData>
        where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal readonly int StartVertex;
        internal int VertexCount;
        internal readonly int StartIndex;
        internal int IndexCount;
        internal TBatchItemData Data;
        internal byte PrimitiveType;

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
            return Data.Equals(ref otherData) && PrimitiveType == primitiveType && (PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip || PrimitiveType != (byte)Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip);
        }
    }
}
