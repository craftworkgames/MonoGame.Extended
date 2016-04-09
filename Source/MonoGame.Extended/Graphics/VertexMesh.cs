using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace MonoGame.Extended.Graphics
{
    public class VertexMesh<TVertexType> : IMesh<TVertexType>
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

        public VertexMesh(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices = null)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            PrimitiveType = primitiveType;
            _vertices = vertices;
            _indices = indices;
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, IDrawContext drawContext = null)
        {
            if (_indices != null)
            {
                primitiveBatch.Draw(PrimitiveType, _vertices, _indices, drawContext);
            }
            else
            {
                primitiveBatch.Draw(PrimitiveType, _vertices, drawContext);
            }
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, int startVertex, int vertexCount, IDrawContext drawContext = null)
        {
            primitiveBatch.Draw(PrimitiveType, _vertices, startVertex, vertexCount, drawContext);
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext = null)
        {
            primitiveBatch.Draw(PrimitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, indexCount, drawContext);
        }
    }
}
