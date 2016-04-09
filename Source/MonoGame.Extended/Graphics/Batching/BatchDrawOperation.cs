using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class BatchDrawOperation
    {
        internal readonly PrimitiveType PrimitiveType; 
        internal readonly int StartVertex;
        internal int VertexCount;
        internal readonly int StartIndex;
        internal int IndexCount;
        internal readonly IDrawContext DrawContext;

        internal BatchDrawOperation(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            PrimitiveType = primitiveType;
            StartVertex = startVertex;
            VertexCount = vertexCount;
            StartIndex = startIndex;
            IndexCount = indexCount;
            DrawContext = drawContext;
        }
    }
}
