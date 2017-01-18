using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public abstract class GraphicsGeometryBuffer<TVertexType, TIndexType> : IDisposable
        where TVertexType : struct, IVertexType where TIndexType : struct
    {
        public TVertexType[] Vertices { get; }
        public int VerticesCount { get; set; }
        public int MaximumVerticesCount { get; }
        public TIndexType[] Indices { get; }
        public int IndicesCount { get; set; }
        public int MaximumIndicesCount { get; }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        protected GraphicsGeometryBuffer(GraphicsDevice graphicsDevice, int maximumVertices, int maximumIndices, bool isDynamic)
        {
            if (graphicsDevice == null)
                throw new NullReferenceException(nameof(graphicsDevice));

            if (maximumVertices < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVertices));
            }

            if (maximumIndices < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndices));
            }

            Vertices = new TVertexType[maximumVertices];
            Indices = new TIndexType[maximumIndices];
            MaximumVerticesCount = maximumVertices;
            MaximumIndicesCount = maximumIndices;

            if (isDynamic)
            {
                VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), maximumVertices, BufferUsage.WriteOnly);
                IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(TIndexType), maximumIndices, BufferUsage.WriteOnly);
            }
            else
            {
                VertexBuffer = new VertexBuffer(graphicsDevice, typeof(TVertexType), maximumVertices, BufferUsage.WriteOnly);
                IndexBuffer = new IndexBuffer(graphicsDevice, typeof(TIndexType), maximumIndices, BufferUsage.WriteOnly);
            }
        }

        protected abstract void CopyIndices(TIndexType[] source, int sourceStartIndex, TIndexType[] destination,
            int desintationStartIndex, int length, int offset);

        public void CopyIndicesTo(TIndexType[] destination, int sourceStartIndex, int desintationStartIndex, int length, int offset)
        {
            CopyIndices(Indices, sourceStartIndex, destination, desintationStartIndex, length, offset);
        }

        public int EnqueueIndicesFrom(TIndexType[] source, int sourceStartIndex, int length, int offset)
        {
            var startIndex = IndicesCount;
            CopyIndices(source, sourceStartIndex, Indices, IndicesCount, length, offset);
            IndicesCount += length;
            return startIndex;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="diposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected void Dispose(bool diposing)
        {
            if (!diposing)
                return;
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
