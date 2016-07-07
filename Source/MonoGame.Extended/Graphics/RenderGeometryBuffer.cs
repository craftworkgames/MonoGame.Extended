using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class RenderGeometryBuffer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        public readonly TVertexType[] Vertices;
        public readonly int[] Indices;

        public int VerticesCount { get; private set; }
        public int IndicesCount { get; private set; }

        public RenderGeometryBuilder.VertexDelegate<TVertexType> EnqueueVertexDelegate { get; }
        public RenderGeometryBuilder.VertexIndexDelegate EnqueueVertexIndexDelegate { get; }

        public RenderGeometryBuffer(int maximumVerticesCount, int maximumIndicesCount)
        {
            if (maximumVerticesCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVerticesCount));
            }

            if (maximumIndicesCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndicesCount));
            }

            Vertices = new TVertexType[maximumIndicesCount];
            Indices = new int[maximumIndicesCount];

            EnqueueVertexDelegate = EnqueueVertex;
            EnqueueVertexIndexDelegate = EnqueueIndex;
        }

        private void EnqueueVertex(ref TVertexType vertexType)
        {
            Vertices[VerticesCount++] = vertexType;
        }

        private void EnqueueIndex(int index)
        {
            Indices[IndicesCount++] = index;
        }

        public void Enqueue(TVertexType[] vertices, int startVertex, int vertexCount, int[] indices, int startIndex, int indexCount)
        {
            var indexOffset = VerticesCount;

            Array.Copy(vertices, startVertex, Vertices, VerticesCount, vertexCount);
            VerticesCount += vertexCount;

            Array.Copy(indices, startIndex, Indices, IndicesCount, indexCount);

            var maxIndexCount = IndicesCount + indexCount;
            while (IndicesCount < maxIndexCount)
            {
                Indices[IndicesCount++] += indexOffset;
            }
        }

        public void Clear()
        {
            VerticesCount = 0;
            IndicesCount = 0;
        }
    }
}
