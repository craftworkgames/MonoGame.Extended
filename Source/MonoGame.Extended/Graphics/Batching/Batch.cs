using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    // why batching? see top answer: http://answers.unity3d.com/questions/14578/whats-the-best-way-to-reduce-draw-calls.html

    /// <summary>
    ///     Enables a group of geometric objects to be drawn using the same settings.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TBatchDrawCommandData">The type of data stored with each draw command.</typeparam>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     <para>
    ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> when instantiating
    ///         <see cref="Batch{TVertexType,TBatchDrawCommandData}" /> for geometry which changes frame-to-frame. For geometry
    ///         which does not change frame-to-frame, or changes infrequently between frames, use
    ///         <see cref="StaticGeometryBuffer{TVertexType}" /> instead.
    ///     </para>
    /// </remarks>
    public abstract class Batch<TVertexType, TBatchDrawCommandData> : IDisposable
        where TVertexType : struct, IVertexType
        where TBatchDrawCommandData : struct, IBatchDrawCommandData<TBatchDrawCommandData>
    {
        internal const int DefaultMaximumBatchCommandsCount = 2048;

        private BatchCommandDrawer<TVertexType, TBatchDrawCommandData> _batchCommandDrawer;
        private BatchCommandQueue<TVertexType, TBatchDrawCommandData> _currentBatchCommandQueue;
        private DeferredBatchCommandQueue<TVertexType, TBatchDrawCommandData> _deferredBatchQueue;
        private ImmediateBatchCommandQueue<TVertexType, TBatchDrawCommandData> _immediateBatchQueue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Batch{TVertexType,TBatchDrawCommandData}" /> class.
        /// </summary>
        /// <param name="geometryBuffer">The geometry buffer.</param>
        /// <param name="maximumBatchCommandsCount">
        ///     The maximum number of batch draw commands that can be deferred. The default value is <code>2048</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="geometryBuffer" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="maximumBatchCommandsCount" /> is less than or equal
        ///     <code>0</code>.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> for geometry which changes
        ///         frame-to-frame and <see cref="StaticGeometryBuffer{TVertexType}" /> for geoemtry which does not change
        ///         frame-to-frame, or changes infrequently between frames.
        ///     </para>
        /// </remarks>
        protected Batch(GeometryBuffer<TVertexType> geometryBuffer,
            int maximumBatchCommandsCount = DefaultMaximumBatchCommandsCount)
        {
            if (geometryBuffer == null)
                throw new ArgumentNullException(nameof(geometryBuffer));

            if (maximumBatchCommandsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumBatchCommandsCount));

            GeometryBuffer = geometryBuffer;
            GraphicsDevice = geometryBuffer.GraphicsDevice;
            _batchCommandDrawer = new BatchCommandDrawer<TVertexType, TBatchDrawCommandData>(GraphicsDevice, GeometryBuffer);

            _immediateBatchQueue = new ImmediateBatchCommandQueue<TVertexType, TBatchDrawCommandData>(GraphicsDevice, _batchCommandDrawer);
            _deferredBatchQueue = new DeferredBatchCommandQueue<TVertexType, TBatchDrawCommandData>(GraphicsDevice, _batchCommandDrawer,
                maximumBatchCommandsCount);
        }

        /// <summary>
        ///     Gets the <see cref="GeometryBuffer{TVertexType}" /> associated with this
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryBuffer{TVertexType}" /> associated with this
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}" />.
        /// </value>
        protected GeometryBuffer<TVertexType> GeometryBuffer { get; }

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="Batch{TVertexType,TBatchDrawCommandData}" />.
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
                GeometryBuffer?.Dispose();

                _batchCommandDrawer?.Dispose();
                _batchCommandDrawer = null;


                _immediateBatchQueue.Dispose();
                _immediateBatchQueue = null;

                _deferredBatchQueue.Dispose();
                _deferredBatchQueue = null;
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
        ///     Starts a group of geometry for rendering with the specified <see cref="Effect" />, <see cref="PrimitiveType" />,
        ///     and <see cref="BatchSortMode" />.
        /// </summary>
        /// <param name="effect">The <see cref="Effect" />.</param>
        /// <param name="primitiveType">The <see cref="PrimitiveType" />.</param>
        /// <param name="sortMode">The <see cref="BatchSortMode" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="effect" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="sortMode" /> is not a valid <see cref="BatchSortMode" /> value or
        ///     <paramref name="primitiveType" /> is not a valid <see cref="PrimitiveType" /> value.
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
        protected void Begin(Effect effect, PrimitiveType primitiveType, BatchSortMode sortMode = BatchSortMode.Deferred)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            if ((primitiveType < PrimitiveType.TriangleList) || (primitiveType > PrimitiveType.LineStrip))
                throw new ArgumentOutOfRangeException(nameof(primitiveType));

            EnsureHasNotBegun();
            HasBegun = true;

            if (sortMode != BatchSortMode.Immediate)
            {
                var deferredQueuer = _deferredBatchQueue;
                deferredQueuer.SortMode = sortMode;
                _currentBatchCommandQueue = deferredQueuer;
            }
            else
            {
                _currentBatchCommandQueue = _immediateBatchQueue;
            }

            _currentBatchCommandQueue.Begin(effect, primitiveType);
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
        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;

            _currentBatchCommandQueue.End();
        }

        /// <summary>
        ///     Submits the group of geometry to the <see cref="GraphicsDevice" /> for rendering without ending the group of
        ///     geometry.
        /// </summary>
        protected void Flush()
        {
            _currentBatchCommandQueue.Flush();
        }

        /// <summary>
        ///     Adds geometry to the group of geometry for rendering using the specified primitive type, vertices, indices, data,
        ///     and optional sort key.
        /// </summary>
        /// <param name="startIndex">The starting index from the <see cref="GeometryBuffer" /> to use.</param>
        /// <param name="primitiveCount">The number of primitives from the <see cref="GeometryBuffer" /> to use.</param>
        /// <param name="itemData">The <see cref="TBatchDrawCommandData" />.</param>
        /// <param name="sortKey">The sort key.</param>
        /// <remarks>
        ///     <para>
        ///         <see cref="Begin" /> must be called before enqueuing any draw calls. When all the geometry have been enqueued
        ///         for drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        protected void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, uint sortKey,
            ref TBatchDrawCommandData itemData)
        {
            EnsureHasBegun();
            _currentBatchCommandQueue.EnqueueDrawCommand(startIndex, primitiveCount, sortKey, ref itemData);
        }
    }
}