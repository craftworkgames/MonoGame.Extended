using System.Runtime.InteropServices;

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

        internal BatchDrawOperation(int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            StartVertex = startVertex;
            VertexCount = vertexCount;
            StartIndex = startIndex;
            IndexCount = indexCount;
            DrawContext = drawContext;
        }
    }
}
