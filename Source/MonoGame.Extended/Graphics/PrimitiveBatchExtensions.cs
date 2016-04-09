using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveBatchExtensions
    {
        public static void DrawVertexMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, VertexMesh<TVertexType> vertexMesh, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            vertexMesh.Draw(primitiveBatch, drawContext);
        }

        public static void DrawVertexMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, VertexMesh<TVertexType> vertexMesh, int startVertex, int vertexCount, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            vertexMesh.Draw(primitiveBatch, startVertex, vertexCount, drawContext);
        }

        public static void DrawVertexMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, VertexMesh<TVertexType> vertexMesh, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            vertexMesh.Draw(primitiveBatch, startVertex, vertexCount, startIndex, indexCount, drawContext);
        }
    }
}
