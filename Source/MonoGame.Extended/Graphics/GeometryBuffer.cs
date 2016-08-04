using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    /// <summary>
    ///     Stores geometry which is to be uploaded to a <see cref="GraphicsDevice" />.
    /// </summary>
    /// <typeparam name="TVertexType">The type of the vertex type.</typeparam>
    /// <seealso cref="IDisposable" />
    public class GeometryBuffer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        //TODO: Add TIndexType to allow for different type of indices: int, short, etc

        private readonly TVertexType[] _vertices;
        private readonly int[] _indices;

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
        public int VertexCount { get; private set; }

        /// <summary>
        ///     Gets the number of buffered indices.
        /// </summary>
        /// <value>
        ///     The number of buffered indices.
        /// </value>
        public int IndexCount { get; private set; }

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

        public GeometryBufferType BufferType { get; }

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
        ///     Initializes a new instance of the <see cref="GeometryBuffer{TVertexType}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="bufferType">The type of buffer.</param>
        /// <param name="maximumVerticesCount">The maximum number of vertices.</param>
        /// <param name="maximumIndicesCount">The maximum number of indices.</param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumVerticesCount" /> is 0, or, <paramref name="maximumVerticesCount" /> is 0, or,
        ///     <paramref name="bufferType" /> is invalid.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         For best performance, use <see cref="GeometryBufferType.Dynamic" /> for geometry which changes frame-to-frame
        ///         and <see cref="GeometryBufferType.Static" /> for geoemtry which does not change frame-to-frame. It is not
        ///         uncommon to have two <see cref="GeometryBuffer{TVertexType}" /> instances for dynamic and
        ///         static geoemtry respectively.
        ///     </para>
        ///     <para>
        ///         Memory will be allocated for the vertex and index array buffers in proportion to
        ///         <paramref name="maximumVerticesCount" /> and <paramref name="maximumIndicesCount" /> respectively.
        ///     </para>
        /// </remarks>
        public GeometryBuffer(GraphicsDevice graphicsDevice, GeometryBufferType bufferType, int maximumVerticesCount, int maximumIndicesCount)
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

            if (bufferType != GeometryBufferType.Static && bufferType != GeometryBufferType.Dynamic)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferType));
            }

            GraphicsDevice = graphicsDevice;

            _vertices = new TVertexType[maximumIndicesCount];
            _indices = new int[maximumIndicesCount];

            BufferType = bufferType;

            switch (bufferType)
            {
                case GeometryBufferType.Static:
                    VertexBuffer = new VertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
                    IndexBuffer = new IndexBuffer(graphicsDevice, typeof(uint), maximumIndicesCount, BufferUsage.WriteOnly);
                    break;
                case GeometryBufferType.Dynamic:
                    VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
                    IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(uint), maximumIndicesCount, BufferUsage.WriteOnly);
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
            VertexBuffer.SetData(_vertices, 0, VertexCount);
            IndexBuffer.SetData(_indices, 0, IndexCount);
        }

        /// <summary>
        ///     Adds the specified vertex to the buffer of geometry.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void Enqueue(TVertexType vertex)
        {
            _vertices[VertexCount++] = vertex;
        }

        /// <summary>
        ///     Adds the specified vertex to the buffer of geometry.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public void Enqueue(ref TVertexType vertex)
        {
            _vertices[VertexCount++] = vertex;
        }

        /// <summary>
        ///     Adds the specified vertex index to the buffer of geometry.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        public void Enqueue(int index)
        {
            _indices[IndexCount++] = index;
        }

        /// <summary>
        ///     Adds the specified vertex index to the buffer of geometry.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        public void Enqueue(ref int index)
        {
            _indices[IndexCount++] = index;
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
            VertexCount += vertexCount;
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
            {
                _indices[IndexCount++] += indexOffset;
            }
        }

        /// <summary>
        ///     Removes all the buffered geometry.
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
        /// <param name="diposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
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
