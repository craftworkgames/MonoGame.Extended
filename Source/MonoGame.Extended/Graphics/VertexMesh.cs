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
        public int PrimitiveCount { get; }

        public int StartIndex { get; set; }
        public int VertexOffset { get; set; }

        internal int VertexBufferIndex { get; set; }

        internal int IndexBufferIndex { get; set; }

        public VertexMesh(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices = null)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            PrimitiveType = primitiveType;
            _vertices = vertices;
            if (indices == null)
            {
                PrimitiveCount = primitiveType.GetPrimitiveCount(vertices.Length);
            }
            else
            {
                _indices = indices;
                PrimitiveCount = primitiveType.GetPrimitiveCount(indices.Length);
            }
        }

        public void Draw(PrimitiveBatch<TVertexType> primitiveBatch, IDrawContext drawContext)
        {
            if (PrimitiveCount <= 0)
            {
                return;
            }

            primitiveBatch.Draw(PrimitiveType, _vertices, _indices, drawContext);
        }
    }
}
