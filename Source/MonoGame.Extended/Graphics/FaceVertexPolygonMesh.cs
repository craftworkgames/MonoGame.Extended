using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public class FaceVertexPolygonMesh<TVertexType> : IPolygonMesh<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private readonly TVertexType[] _vertices;
        private readonly short[] _indices;

        public IReadOnlyCollection<TVertexType> Vertices
        {
            get { return _vertices; }
        }

        public IReadOnlyCollection<short> Indices
        {
            get { return _indices; }
        }

        public PrimitiveType PrimitiveType { get; }

        public FaceVertexPolygonMesh(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices = null)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            PrimitiveType = primitiveType;
            _vertices = vertices;
            _indices = indices;
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, uint sortKey)
        {
            if (_indices != null)
            {
                primitiveBatch.Draw(PrimitiveType, _vertices, _indices, sortKey);
            }
            else
            {
                primitiveBatch.Draw(PrimitiveType, _vertices, sortKey);
            }
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, int startVertex, int vertexCount, uint sortKey)
        {
            primitiveBatch.Draw(PrimitiveType, _vertices, startVertex, vertexCount, sortKey);
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, int startVertex, int vertexCount, int startIndex, int indexCount, uint sortKey)
        {
            primitiveBatch.Draw(PrimitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, indexCount, sortKey);
        }
    }
}
