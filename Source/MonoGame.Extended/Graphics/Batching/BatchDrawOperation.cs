using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BatchDrawOperation
    {
        internal readonly int StartVertex;
        internal int VertexCount;
        internal readonly int StartIndex;
        internal int IndexCount;
        internal IDrawContext DrawContext;
        internal byte PrimitiveType;

        internal BatchDrawOperation(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            PrimitiveType = (byte)primitiveType;
            StartVertex = startVertex;
            VertexCount = vertexCount;
            StartIndex = startIndex;
            IndexCount = indexCount;
            DrawContext = drawContext;
        }
    }
}
