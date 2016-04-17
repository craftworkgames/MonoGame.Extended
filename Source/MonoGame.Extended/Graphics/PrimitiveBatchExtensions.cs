using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public static class PrimitiveBatchExtensions
    {
        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, uint sortKey = 0) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, sortKey);
        }

        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, int startVertex, int vertexCount, uint sortKey = 0) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, startVertex, vertexCount, sortKey);
        }

        public static void DrawPolygonMesh<TVertexType>(this PrimitiveBatch<TVertexType> primitiveBatch, FaceVertexPolygonMesh<TVertexType> faceVertexPolygonMesh, int startVertex, int vertexCount, int startIndex, int indexCount, uint sortKey = 0) where TVertexType : struct, IVertexType
        {
            faceVertexPolygonMesh.Draw(primitiveBatch, startVertex, vertexCount, startIndex, indexCount, sortKey);
        }
    }
}
