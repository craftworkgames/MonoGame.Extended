using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class MeshBuffer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        private readonly TVertexType[] _vertices;
        private readonly int[] _indices;

        public IReadOnlyList<TVertexType> Vertices
        {
            get { return _vertices; }
        }

        public IReadOnlyList<int> Indices
        {
            get { return _indices;  }
        }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public OutputVertexDelegate<TVertexType> EnqueueVertexDelegate { get; }
        public OutputVertexIndexDelegate EnqueueIndexDelegate { get; }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public MeshBufferType Type { get; }

        public MeshBuffer(GraphicsDevice graphicsDevice, MeshBufferType type, int maximumVerticesCount, int maximumIndicesCount)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            if (maximumVerticesCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVerticesCount));
            }

            if (maximumIndicesCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndicesCount));
            }

            if (type > MeshBufferType.Dynamic)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            _vertices = new TVertexType[maximumIndicesCount];
            _indices = new int[maximumIndicesCount];

            Type = type;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case MeshBufferType.Static:
                    VertexBuffer = new VertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
                    IndexBuffer = new IndexBuffer(graphicsDevice, typeof(uint), maximumIndicesCount, BufferUsage.WriteOnly);
                    break;
                case MeshBufferType.Dynamic:
                    VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(uint), maximumIndicesCount, BufferUsage.WriteOnly);
                    break;
            }

            EnqueueVertexDelegate = Enqueue;
            EnqueueIndexDelegate = Enqueue;
        }

        public void Flush()
        {
            VertexBuffer.SetData(_vertices, 0, VertexCount);
            IndexBuffer.SetData(_indices, 0, IndexCount);
        }

        public void Enqueue(TVertexType vertex)
        {
            _vertices[VertexCount++] = vertex;
        }

        public void Enqueue(ref TVertexType vertex)
        {
            _vertices[VertexCount++] = vertex;
        }

        public void Enqueue(int index)
        {
            _indices[IndexCount++] = index;
        }

        public void Enqueue(ref int index)
        {
            _indices[IndexCount++] = index;
        }

        public void Enqueue(TVertexType[] vertices, int startVertex, int vertexCount)
        {
            Array.Copy(vertices, startVertex, _vertices, VertexCount, vertexCount);
            VertexCount += vertexCount;
        }

        public void Enqueue(TVertexType[] vertices, int startVertex, int vertexCount, uint[] indices, int startIndex, int indexCount)
        {
            var indexOffset = VertexCount;

            Enqueue(vertices, startVertex, vertexCount);

            Array.Copy(indices, startIndex, _indices, IndexCount, indexCount);

            var maxIndexCount = IndexCount + indexCount;
            while (IndexCount < maxIndexCount)
            {
                _indices[IndexCount++] += indexOffset;
            }
        }

        public void Clear()
        {
            VertexCount = 0;
            IndexCount = 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
            {
                return;
            }

            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
