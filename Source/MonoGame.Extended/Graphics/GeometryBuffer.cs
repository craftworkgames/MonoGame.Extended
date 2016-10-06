using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Stores geometry which is to be uploaded to a <see cref="GraphicsDevice" />.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TIndexType">The type of vertex index.</typeparam>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     <para>
    ///         For best performance, use <see cref="GeometryBufferType.Dynamic" /> for geometry which changes
    ///         frame-to-frame and <see cref="GeometryBufferType.Static" /> for geoemtry which does not change
    ///         frame-to-frame, or changes infrequently between frames.
    ///     </para>
    /// </remarks>
    public abstract partial class GeometryBuffer<TVertexType, TIndexType> : IDisposable
        where TVertexType : struct, IVertexType where TIndexType : struct
    {
        /// <summary>
        ///     The buffered vertices.
        /// </summary>
        protected internal readonly TVertexType[] Vertices;

        /// <summary>
        ///     The buffered indices.
        /// </summary>
        protected internal readonly TIndexType[] Indices;

        /// <summary>
        ///     The number of buffered vertices.
        /// </summary>
        protected internal int VertexCount;

        /// <summary>
        ///     The number of buffered indices.
        /// </summary>
        protected internal int IndexCount;

        /// <summary>
        ///     Gets the <see cref="GeometryBufferType" /> associated with this
        ///     <see cref="GeometryBuffer{TVertexType, TIndexType}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryBufferType" /> associated with this <see cref="GeometryBuffer{TVertexType, TIndexType}" />.
        /// </value>
        public GeometryBufferType BufferType { get; }

        /// <summary>
        ///     Gets the maximum number of vertices.
        /// </summary>
        /// <value>
        ///     The maximum number of vertices.
        /// </value>
        public int MaximumVerticesCount { get; }

        /// <summary>
        ///     Gets the maximum number of indices.
        /// </summary>
        /// <value>
        ///     The maximum number of indices.
        /// </value>
        public int MaximumIndicesCount { get; }

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
        ///     <see cref="GeometryBuffer{TVertexType, TIndexType}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="GeometryBuffer{TVertexType, TIndexType}" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeometryBuffer{TVertexType, TIndexType}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice" />.</param>
        /// <param name="bufferType">The <see cref="GeometryBufferType" />.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="graphicsDevice" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>, or,
        ///     <paramref name="maximumVerticesCount" /> is less than or equal to <code>0</code>.
        /// </exception>
        /// <exception cref="InvalidOperationException">The index type is invalid.</exception>
        protected GeometryBuffer(GraphicsDevice graphicsDevice, GeometryBufferType bufferType,
            int maximumVerticesCount,
            int maximumIndicesCount)
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
            BufferType = bufferType;
            Vertices = new TVertexType[maximumVerticesCount];
            Indices = new TIndexType[maximumIndicesCount];
            VertexCount = 0;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            var indexType = typeof(TIndexType);

            if ((indexType != typeof(byte)) && (indexType != typeof(sbyte)) && (indexType != typeof(ushort)) &&
                (indexType != typeof(short)) && (indexType != typeof(uint)) && (indexType != typeof(int)) &&
                (indexType != typeof(ulong)) && (indexType != typeof(long)))
                throw new InvalidOperationException($"The type '{indexType}' is not a valid index type.");

            switch (bufferType)
            {
                case GeometryBufferType.Static:
                    VertexBuffer = new VertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount,
                        BufferUsage.WriteOnly);
                    IndexBuffer = new IndexBuffer(graphicsDevice, indexType, maximumIndicesCount,
                        BufferUsage.WriteOnly);
                    break;
                case GeometryBufferType.Dynamic:
                    VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount,
                        BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(graphicsDevice, indexType, maximumIndicesCount,
                        BufferUsage.WriteOnly);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bufferType));
            }
        }

        /// <summary>
        ///     Uploads the buffered geometry to the <see cref="GraphicsDevice" />.
        /// </summary>
        public void Flush()
        {
            VertexBuffer.SetData(Vertices, 0, VertexCount);
            IndexBuffer.SetData(Indices, 0, IndexCount);
        }

        /// <summary>
        ///     Resets the buffered geometry count.
        /// </summary>
        public void Clear()
        {
            VertexCount = 0;
            IndexCount = 0;
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
        public void ThrowIfWouldOverflowVertices(int verticesCountToAdd)
        {
            if (VertexCount + verticesCountToAdd > MaximumVerticesCount)
                throw new GeometryBufferOverflowException<TVertexType, TIndexType>(this, verticesCountToAdd, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfWouldOverflowIndices(int indicesCountToAdd)
        {
            if (IndexCount + indicesCountToAdd > MaximumIndicesCount)
                throw new GeometryBufferOverflowException<TVertexType, TIndexType>(this, 0, indicesCountToAdd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ThrowIfWouldOverflow(int verticesCountToAdd, int indicesCountToAdd)
        {
            if ((VertexCount + verticesCountToAdd > MaximumVerticesCount) ||
                (IndexCount + indicesCountToAdd > MaximumIndicesCount))
                throw new GeometryBufferOverflowException<TVertexType, TIndexType>(this, verticesCountToAdd,
                    indicesCountToAdd);
        }
    }
}
