using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    internal enum GeometryBufferType
    {
        Static,
        Dynamic
    }

    /// <summary>
    ///     Stores geometry which is to be uploaded to a <see cref="GraphicsDevice" />.
    /// </summary>
    /// <typeparam name="TVertexType">The type of the vertex type.</typeparam>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     <para>
    ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> for geometry which changes
    ///         frame-to-frame and <see cref="StaticGeometryBuffer{TVertexType}" /> for geoemtry which does not change
    ///         frame-to-frame, or changes infrequently between frames.
    ///     </para>
    /// </remarks>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public abstract class GeometryBuffer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        //TODO: Add TIndexType to allow for different type of indices: int, short, etc

        // ReSharper disable InconsistentNaming
        internal readonly TVertexType[] _vertices;
        internal readonly int[] _indices;
        internal int _vertexCount;
        internal int _indexCount;
        // ReSharper restore InconsistentNaming

        internal readonly GeometryBufferType BufferType;

        /// <summary>
        ///     Gets the vertices as read-only.
        /// </summary>
        /// <value>
        ///     The vertices as read-only.
        /// </value>
        public IReadOnlyList<TVertexType> Vertices
        {
            get { return _vertices; }
        }

        /// <summary>
        ///     Gets the indices as read-only.
        /// </summary>
        /// <value>
        ///     The indices as read-only.
        /// </value>
        public IReadOnlyList<int> Indices
        {
            get { return _indices; }
        }

        /// <summary>
        ///     Gets the number of buffered vertices.
        /// </summary>
        /// <value>
        ///     The number of buffered vertices.
        /// </value>
        public int VertexCount
        {
            get { return _vertexCount; }
        }

        /// <summary>
        ///     Gets the number of buffered indices.
        /// </summary>
        /// <value>
        ///     The number of buffered indices.
        /// </value>
        public int IndexCount
        {
            get { return _indexCount; }
        }

        /// <summary>
        ///     Gets the vertex buffer.
        /// </summary>
        /// <value>
        ///     The vertex buffer.
        /// </value>
        public VertexBuffer VertexBuffer { get; }

        /// <summary>
        ///     Gets the index buffer.
        /// </summary>
        /// <value>
        ///     The index buffer.
        /// </value>
        public IndexBuffer IndexBuffer { get; }

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="GeometryBuffer{TVertexType}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="GeometryBuffer{TVertexType}" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        internal GeometryBuffer(GraphicsDevice graphicsDevice, GeometryBufferType bufferType, int maximumVerticesCount, int maximumIndicesCount)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(paramName: nameof(graphicsDevice));

            if (maximumVerticesCount <= 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(maximumVerticesCount));

            if (maximumIndicesCount <= 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(maximumIndicesCount));

            if ((bufferType != GeometryBufferType.Static) && (bufferType != GeometryBufferType.Dynamic))
                throw new ArgumentOutOfRangeException(paramName: nameof(bufferType));

            GraphicsDevice = graphicsDevice;

            _vertices = new TVertexType[maximumIndicesCount];
            _indices = new int[maximumIndicesCount];
            _vertexCount = 0;

            BufferType = bufferType;

            switch (bufferType)
            {
                case GeometryBufferType.Static:
                    VertexBuffer = new VertexBuffer(graphicsDevice, type: typeof(TVertexType), vertexCount: maximumVerticesCount, bufferUsage: BufferUsage.WriteOnly);
                    IndexBuffer = new IndexBuffer(graphicsDevice, indexType: typeof(uint), indexCount: maximumIndicesCount, usage: BufferUsage.WriteOnly);
                    break;
                case GeometryBufferType.Dynamic:
                    VertexBuffer = new DynamicVertexBuffer(graphicsDevice, type: typeof(TVertexType), vertexCount: maximumVerticesCount, bufferUsage: BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(graphicsDevice, indexType: typeof(uint), indexCount: maximumIndicesCount, usage: BufferUsage.WriteOnly);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(paramName: nameof(bufferType));
            }
        }

        /// <summary>
        ///     Uploads the buffered geometry to the <see cref="GraphicsDevice" />.
        /// </summary>
        public void Flush()
        {
            VertexBuffer.SetData(_vertices, startIndex: 0, elementCount: VertexCount);
            IndexBuffer.SetData(_indices, startIndex: 0, elementCount: IndexCount);
        }

        /// <summary>
        ///     Adds the specified vertex to the buffer of geometry.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void Enqueue(ref TVertexType vertex)
        {
            _vertices[_vertexCount++] = vertex;
        }

        /// <summary>
        ///     Adds the specified vertex index to the buffer of geometry.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        public void Enqueue(int index)
        {
            _indices[_indexCount++] = index;
        }

        /// <summary>
        ///     Adds the specified vertices to the buffer of geometry.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="startVertex">
        ///     The value that represents the index in the <paramref name="vertices" /> at which adding
        ///     begins.
        /// </param>
        /// <param name="vertexCount">The number of vertices to add.</param>
        public void Enqueue(TVertexType[] vertices, int startVertex, int vertexCount)
        {
            Array.Copy(vertices, startVertex, _vertices, VertexCount, vertexCount);
            _vertexCount += vertexCount;
        }

        /// <summary>
        ///     Adds the specified vertices and vertex indices to the buffer of geometry.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="startVertex">
        ///     The value that represents the index in the <paramref name="vertices" /> at which adding
        ///     begins.
        /// </param>
        /// <param name="vertexCount">The number of vertices to add.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="startIndex">The value that represents the index in the <paramref name="indices" /> at which adding begins.</param>
        /// <param name="indexCount">The number of indices to add.</param>
        public void Enqueue(TVertexType[] vertices, int startVertex, int vertexCount, uint[] indices, int startIndex, int indexCount)
        {
            var indexOffset = VertexCount;

            Enqueue(vertices, startVertex, vertexCount);

            Array.Copy(indices, startIndex, _indices, IndexCount, indexCount);

            var maxIndexCount = IndexCount + indexCount;
            while (IndexCount < maxIndexCount)
                _indices[_indexCount++] += indexOffset;
        }

        public void EnqueueClockwiseQuadrilateralIndices(int indexOffset)
        {
            Enqueue(index: 0 + indexOffset);
            Enqueue(index: 1 + indexOffset);
            Enqueue(index: 2 + indexOffset);
            Enqueue(index: 1 + indexOffset);
            Enqueue(index: 3 + indexOffset);
            Enqueue(index: 2 + indexOffset);
        }

        /// <summary>
        ///     Resets the buffered geometry count.
        /// </summary>
        public void Clear()
        {
            _vertexCount = 0;
            _indexCount = 0;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
