using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics.Batching
{
    // why batching? see top answer: http://answers.unity3d.com/questions/14578/whats-the-best-way-to-reduce-draw-calls.html

    /// <summary>
    ///     Enables a group of geometric objects to be drawn using the same settings.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TIndexType">The type of vertex index.</typeparam>
    /// <typeparam name="TBatchDrawCommandData">The type of data stored with each draw command.</typeparam>
    /// <seealso cref="IDisposable" />
    public abstract class BatchRenderer<TVertexType, TIndexType, TBatchDrawCommandData> : IDisposable
        where TVertexType : struct, IVertexType
        where TIndexType : struct
        where TBatchDrawCommandData : struct, IBatchDrawCommandData<TBatchDrawCommandData>
    {
        internal const int DefaultMaximumBatchCommandsCount = 2048;

        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private BatchCommandQueue<TVertexType, TIndexType, TBatchDrawCommandData> _currentCommandQueue;
        private readonly DeferredBatchCommandQueue<TVertexType, TIndexType, TBatchDrawCommandData> _deferredCommandQueue;
        private readonly ImmediateBatchCommandQueue<TVertexType, TIndexType, TBatchDrawCommandData> _immediateCommandQueue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" /> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer.</param>
        /// <param name="geometryData">The geometry data buffer.</param>
        /// <param name="maximumBatchCommandsCount">
        ///     The maximum number of batch draw commands that can be deferred. The default value is <code>2048</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="graphicsDevice"/>, or <paramref name="vertexBuffer" />, or <paramref name="indexBuffer"/>, or <paramref name="geometryData"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumBatchCommandsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        protected BatchRenderer(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, GraphicsGeometryData<TVertexType, TIndexType> geometryData,
            int maximumBatchCommandsCount = DefaultMaximumBatchCommandsCount)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(nameof(graphicsDevice));

            if (vertexBuffer == null)
                throw new ArgumentNullException(nameof(vertexBuffer));

            if (indexBuffer == null)
                throw new ArgumentNullException(nameof(indexBuffer));

            if (geometryData == null)
                throw new ArgumentNullException(nameof(geometryData));

            if (maximumBatchCommandsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumBatchCommandsCount));

            GraphicsDevice = graphicsDevice;
            GeometryData = geometryData;
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;

            var commandDrawer = new BatchCommandDrawer<TVertexType, TIndexType, TBatchDrawCommandData>(GraphicsDevice, vertexBuffer, indexBuffer);

            _immediateCommandQueue = new ImmediateBatchCommandQueue<TVertexType, TIndexType, TBatchDrawCommandData>(GraphicsDevice,
                commandDrawer, GeometryData);
            _deferredCommandQueue = new DeferredBatchCommandQueue<TVertexType, TIndexType, TBatchDrawCommandData>(GraphicsDevice,
                commandDrawer, GeometryData,
                maximumBatchCommandsCount);
        }

        /// <summary>
        ///     Gets the <see cref="GeometryData" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryData" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </value>
        protected GraphicsGeometryData<TVertexType, TIndexType> GeometryData { get; }

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="BatchRenderer{TVertexType,TIndexType,TBatchDrawCommandData}" />.
        /// </value>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        ///     Gets a value indicating whether batching is currently in progress by being within a <see cref="Begin" /> and
        ///     <see cref="End" /> pair block of code.
        /// </summary>
        /// <value>
        ///     <c>true</c> if batching has begun; otherwise, <c>false</c>.
        /// </value>
        public bool HasBegun { get; private set; }

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
            // ReSharper disable once InvertIf
            if (diposing)
            {
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasBegun([CallerMemberName] string callerMemberName = null)
        {
            if (!HasBegun)
                throw new InvalidOperationException(
                    $"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
                throw new InvalidOperationException(
                    $"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
        }

        /// <summary>
        ///     Starts a group of geometry for rendering with the specified <see cref="Effect" /> and <see cref="BatchSortMode" />.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" />.</param>
        /// <param name="sortMode">The <see cref="BatchSortMode" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="effect" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="sortMode" /> is not a valid <see cref="BatchSortMode" /> value or
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Begin" /> cannot be invoked again until <see cref="End" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called before any enqueuing of draw calls. When all the geometry have been enqueued for
        ///         drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        protected void Begin(Effect effect, BatchSortMode sortMode = BatchSortMode.Deferred)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            EnsureHasNotBegun();
            HasBegun = true;

            if (sortMode != BatchSortMode.Immediate)
            {
                var deferredQueuer = _deferredCommandQueue;
                deferredQueuer.SortMode = sortMode;
                _currentCommandQueue = deferredQueuer;
            }
            else
            {
                _currentCommandQueue = _immediateCommandQueue;
            }

            _currentCommandQueue.Begin(effect);
        }

        /// <summary>
        ///     Ends and submits the group of geometry to the <see cref="GraphicsDevice" /> for rendering.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="End" /> cannot be invoked until <see cref="Begin" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called after all enqueuing of draw calls.
        ///     </para>
        /// </remarks>
        public virtual void End()
        {
            EnsureHasBegun();
            HasBegun = false;

            _currentCommandQueue.End();
        }

        /// <summary>
        ///     Submits the group of geometry to the <see cref="GraphicsDevice" /> for rendering without ending the group of
        ///     geometry.
        /// </summary>
        protected virtual void Flush()
        {
            _currentCommandQueue.Flush();
        }

        /// <summary>
        ///     Adds geometry to the group of geometry for rendering using the specified primitive type, vertices, indices, data,
        ///     and optional sort key.
        /// </summary>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="startIndex">The starting index from the <see cref="GeometryData" /> to use.</param>
        /// <param name="primitiveCount">The number of primitives from the <see cref="GeometryData" /> to use.</param>
        /// <param name="sortKey">The sort key.</param>
        /// <param name="itemData">The <see cref="TBatchDrawCommandData" />.</param>
        /// <exception cref="InvalidOperationException">The <see cref="Begin" /> method has not been called.</exception>
        /// <remarks>
        ///     <para>
        ///         <see cref="Begin" /> must be called before enqueuing any draw calls. When all the geometry have been enqueued
        ///         for drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        protected void Draw(PrimitiveType primitiveType, int startIndex, int primitiveCount, float sortKey, ref TBatchDrawCommandData itemData)
        {
            EnsureHasBegun();

            _currentCommandQueue.EnqueueDrawCommand(primitiveType, primitiveCount, startIndex, sortKey, ref itemData);
        }
    }
}