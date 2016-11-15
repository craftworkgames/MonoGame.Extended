using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        internal readonly GeometryBufferType BufferType;

        internal GeometryBuffer(GraphicsDevice graphicsDevice, GeometryBufferType bufferType,
            ushort maximumVerticesCount, ushort maximumIndicesCount)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            if (maximumVerticesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumVerticesCount));

            if (maximumIndicesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumIndicesCount));

            if ((bufferType != GeometryBufferType.Static) && (bufferType != GeometryBufferType.Dynamic))
                throw new ArgumentOutOfRangeException(nameof(bufferType));

            GraphicsDevice = graphicsDevice;

            _vertices = new TVertexType[maximumVerticesCount];
            _indices = new ushort[maximumIndicesCount];
            _vertexCount = 0;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            BufferType = bufferType;

            switch (bufferType)
            {
                case GeometryBufferType.Static:
                    VertexBuffer = new VertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount,
                        BufferUsage.WriteOnly);
                    IndexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), maximumIndicesCount,
                        BufferUsage.WriteOnly);
                    break;
                case GeometryBufferType.Dynamic:
                    VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount,
                        BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(ushort), maximumIndicesCount,
                        BufferUsage.WriteOnly);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bufferType));
            }
        }

        /// <summary>
        ///     Gets the vertices as read-only.
        /// </summary>
        /// <value>
        ///     The vertices as read-only.
        /// </value>
        public IReadOnlyList<TVertexType> Vertices => _vertices;

        /// <summary>
        ///     Gets the indices as read-only.
        /// </summary>
        /// <value>
        ///     The indices as read-only.
        /// </value>
        public IReadOnlyList<ushort> Indices => _indices;

        /// <summary>
        ///     Gets the number of buffered vertices.
        /// </summary>
        /// <value>
        ///     The number of buffered vertices.
        /// </value>
        public ushort VertexCount => _vertexCount;

        /// <summary>
        ///     Gets the number of buffered indices.
        /// </summary>
        /// <value>
        ///     The number of buffered indices.
        /// </value>
        public ushort IndexCount => _indexCount;

        public ushort MaximumVerticesCount { get; }
        public ushort MaximumIndicesCount { get; }

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

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Uploads the buffered geometry to the <see cref="GraphicsDevice" />.
        /// </summary>
        public void Flush()
        {
            VertexBuffer.SetData(_vertices, 0, _vertexCount);
            IndexBuffer.SetData(_indices, 0, _indexCount);
        }

        /// <summary>
        ///     Adds the specified vertex to the buffer of geometry.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public bool Enqueue(ref TVertexType vertex)
        {
            ThrowIfWouldOverflowVertices(1);

            _vertices[_vertexCount++] = vertex;

            return true;
        }

        /// <summary>
        ///     Adds the specified vertex index to the buffer of geometry.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        /// <exception cref="GeometryBufferOverflowException{TVertexType}">The <see cref="GeometryBuffer{TVertexType}" /> is full.</exception>
        public void Enqueue(ushort index)
        {
            ThrowIfWouldOverflowIndices(1);
            _indices[_indexCount++] = index;
        }

        /// <summary>
        ///     Attempts to add the specified vertices to the buffer of geometry.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="sourceIndex">
        ///     The value that represents the index in the <paramref name="vertices" /> at which adding
        ///     begins.
        /// </param>
        /// <param name="length">The number of vertices to add.</param>
        /// <exception cref="GeometryBufferOverflowException{TVertexType}">The <see cref="GeometryBuffer{TVertexType}" /> is full.</exception>
        public void Enqueue(TVertexType[] vertices, int sourceIndex, ushort length)
        {
            ThrowIfWouldOverflowVertices(length);

            Array.Copy(vertices, sourceIndex, _vertices, VertexCount, length);
            _vertexCount += length;
        }

        /// <summary>
        ///     Adds vertex indices to the buffer of geometry.
        /// </summary>
        /// <param name="indices">The indices.</param>
        /// <param name="sourceIndex">
        ///     The value that represents the index in the <paramref name="indices" /> at which adding
        ///     begins.
        /// </param>
        /// <param name="length">The number of indices to add.</param>
        /// <param name="indexOffset">The index offset.</param>
        /// <exception cref="GeometryBufferOverflowException{TVertexType}">The <see cref="GeometryBuffer{TVertexType}" /> is full.</exception>
        public unsafe void Enqueue(ushort[] indices, int sourceIndex, ushort length, ushort indexOffset)
        {
            ThrowIfWouldOverflowIndices(length);

            var start = _indexCount;
            var end = _indexCount + length;

            Array.Copy(indices, sourceIndex, _indices, IndexCount, length);

            fixed (ushort* fixedPointer = _indices)
            {
                var pointer = fixedPointer + start;
                var endPointer = fixedPointer + end;

                while (pointer < endPointer)
                {
                    *pointer += indexOffset;
                    ++pointer;
                }
            }

            _indexCount += length;
        }

        /// <summary>
        ///     Adds clockwise quadrilateral indices to the buffer of geometry.
        /// </summary>
        /// <param name="indexOffset">The index offset.</param>
        /// <exception cref="GeometryBufferOverflowException{TVertexType}">The <see cref="GeometryBuffer{TVertexType}" /> is full.</exception>
        public unsafe void EnqueueClockwiseQuadrilateralIndices(ushort indexOffset)
        {
            ThrowIfWouldOverflowIndices(6);

            fixed (ushort* fixedPointer = _indices)
            {
                var pointer = fixedPointer + _indexCount;
                *(pointer + 0) = (ushort) (0 + indexOffset);
                *(pointer + 1) = (ushort) (1 + indexOffset);
                *(pointer + 2) = (ushort) (2 + indexOffset);
                *(pointer + 3) = (ushort) (1 + indexOffset);
                *(pointer + 4) = (ushort) (3 + indexOffset);
                *(pointer + 5) = (ushort) (2 + indexOffset);
            }
            _indexCount += 6;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfWouldOverflowVertices(ushort verticesCountToAdd)
        {
            if (_vertexCount + verticesCountToAdd > MaximumVerticesCount)
                throw new GeometryBufferOverflowException<TVertexType>(this, verticesCountToAdd, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfWouldOverflowIndices(ushort indicesCountToAdd)
        {
            if (_indexCount + indicesCountToAdd > MaximumIndicesCount)
                throw new GeometryBufferOverflowException<TVertexType>(this, 0, indicesCountToAdd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ThrowIfWouldOverflow(ushort verticesCountToAdd, ushort indicesCountToAdd)
        {
            if ((_vertexCount + verticesCountToAdd > MaximumVerticesCount) ||
                (_indexCount + indicesCountToAdd > MaximumIndicesCount))
                throw new GeometryBufferOverflowException<TVertexType>(this, verticesCountToAdd, indicesCountToAdd);
        }

        //TODO: Add TIndexType to allow for different type of indices: int, short, etc

        // ReSharper disable InconsistentNaming
        internal readonly TVertexType[] _vertices;
        internal readonly ushort[] _indices;
        internal ushort _vertexCount;
        internal ushort _indexCount;
        // ReSharper restore InconsistentNaming
    }
}