using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveBatchExtensions
    {
        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, drawContext);
        }

        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, int startVertex, int vertexCount, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, startVertex, vertexCount, drawContext);
        }

        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext = null) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, startVertex, vertexCount, startIndex, indexCount, drawContext);
        }
    }
}
