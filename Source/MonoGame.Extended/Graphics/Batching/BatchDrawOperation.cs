using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BatchDrawOperation
    {
        internal readonly uint SortKey;
        internal readonly int StartVertex;
        internal int VertexCount;
        internal readonly int StartIndex;
        internal int IndexCount;
        internal readonly byte PrimitiveType;

        internal BatchDrawOperation(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, uint sortKey)
        {
            PrimitiveType = (byte)primitiveType;
            StartVertex = startVertex;
            VertexCount = vertexCount;
            StartIndex = startIndex;
            IndexCount = indexCount;
            SortKey = sortKey;
        }
    }
}
