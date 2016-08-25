using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching.Drawers;
using MonoGame.Extended.Graphics.Batching.Queuers;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Enables a group of geometry to be drawn using the same settings.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TCommandContext">The type of batch draw command data.</typeparam>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     <para>
    ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> when instantiating
    ///         <see cref="PrimitiveBatch{TVertexType,TDrawContext}" /> for geometry which changes frame-to-frame. For geometry
    ///         which does not change frame-to-frame, or changes infrequently between frames, use
    ///         <see cref="StaticGeometryBuffer{TVertexType}" /> when instantiating
    ///         <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />.
    ///     </para>
    /// </remarks>
    public abstract class PrimitiveBatch<TVertexType, TCommandContext> : IDisposable
        where TVertexType : struct, IVertexType where TCommandContext : struct, IBatchCommandContext
    {
        internal const int DefaultMaximumBatchCommandsCount = 2048;

        private BatchDrawer<TVertexType, TCommandContext> _batchDrawer;
        private BatchCommandQueuer<TVertexType, TCommandContext> _currentBatchCommandQueuer;
        private Lazy<ImmediateBatchCommandQueuer<TVertexType, TCommandContext>> _lazyImmediateBatchQueuer;
        private Lazy<DeferredBatchCommandQueuer<TVertexType, TCommandContext>> _lazyDeferredBatchQueuer;

        /// <summary>
        ///     Gets the <see cref="GeometryBuffer{TVertexType}" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryBuffer{TVertexType}" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />.
        /// </value>
        protected GeometryBuffer<TVertexType> GeometryBuffer { get; }

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TDrawContext}" />.
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
        ///     Initializes a new instance of the <see cref="PrimitiveBatch{TVertexType, TDrawContext}" /> class.
        /// </summary>
        /// <param name="geometryBuffer">The geometry buffer.</param>
        /// <param name="maximumBatchCommandsCount">
        ///     The maximum number of batch draw commands that can be deferred. The default value is <code>2048</code>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="geometryBuffer" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumBatchCommandsCount" /> is less than or equal <code>0</code>.</exception>
        /// <remarks>
        ///     <para>
        ///         For best performance, use <see cref="DynamicGeometryBuffer{TVertexType}" /> for geometry which changes
        ///         frame-to-frame and <see cref="StaticGeometryBuffer{TVertexType}" /> for geoemtry which does not change
        ///         frame-to-frame, or changes infrequently between frames.
        ///     </para>
        /// </remarks>
        protected PrimitiveBatch(GeometryBuffer<TVertexType> geometryBuffer, int maximumBatchCommandsCount = DefaultMaximumBatchCommandsCount)
        {
            if (geometryBuffer == null)
                throw new ArgumentNullException(paramName: nameof(geometryBuffer));

            if (maximumBatchCommandsCount <= 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(maximumBatchCommandsCount));

            GeometryBuffer = geometryBuffer;
            GraphicsDevice = geometryBuffer.GraphicsDevice;
            _batchDrawer = new BatchDrawer<TVertexType, TCommandContext>(GraphicsDevice, GeometryBuffer);

            _lazyImmediateBatchQueuer = new Lazy<ImmediateBatchCommandQueuer<TVertexType, TCommandContext>>(() => new ImmediateBatchCommandQueuer<TVertexType, TCommandContext>(_batchDrawer));
            _lazyDeferredBatchQueuer = new Lazy<DeferredBatchCommandQueuer<TVertexType, TCommandContext>>(() => new DeferredBatchCommandQueuer<TVertexType, TCommandContext>(_batchDrawer, maximumBatchCommandsCount));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(diposing: true);
            GC.SuppressFinalize(obj: this);
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

                _batchDrawer?.Dispose();
                _batchDrawer = null;

                if (_lazyImmediateBatchQueuer?.IsValueCreated ?? false)
                {
                    _lazyImmediateBatchQueuer.Value.Dispose();
                    _lazyImmediateBatchQueuer = null;
                }

                // ReSharper disable once InvertIf
                if (_lazyDeferredBatchQueuer?.IsValueCreated ?? false)
                {
                    _lazyDeferredBatchQueuer.Value.Dispose();
                    _lazyDeferredBatchQueuer = null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasBegun([CallerMemberName] string callerMemberName = null)
        {
            if (!HasBegun)
                throw new InvalidOperationException(message: $"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
                throw new InvalidOperationException(message: $"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
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
                throw new ArgumentNullException(paramName: nameof(effect));

            if ((primitiveType < PrimitiveType.TriangleList) || (primitiveType > PrimitiveType.LineStrip))
                throw new ArgumentOutOfRangeException(paramName: nameof(primitiveType));

            EnsureHasNotBegun();
            HasBegun = true;

            if (sortMode != BatchSortMode.Immediate)
            {
                var deferredQueuer = _lazyDeferredBatchQueuer.Value;
                deferredQueuer.SortMode = sortMode;
                _currentBatchCommandQueuer = deferredQueuer;
            }
            else
            {
                _currentBatchCommandQueuer = _lazyImmediateBatchQueuer.Value;
            }

            _currentBatchCommandQueuer.Begin(effect, primitiveType);
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

            _currentBatchCommandQueuer.End();
        }

        /// <summary>
        ///     Submits the group of geometry to the <see cref="GraphicsDevice" /> for rendering without ending the group of
        ///     geometry.
        /// </summary>
        protected void Flush()
        {
            _currentBatchCommandQueuer.Flush();
        }

        /// <summary>
        ///     Adds geometry to the group of geometry for rendering using the specified primitive type, vertices, indices, data,
        ///     and optional sort key.
        /// </summary>
        /// <param name="startIndex">The starting index from the <see cref="GeometryBuffer" /> to use.</param>
        /// <param name="indexCount">The number of indices from the <see cref="GeometryBuffer" /> to use.</param>
        /// <param name="itemData">The custom data for the draw operation.</param>
        /// <param name="sortKey">The sort key.</param>
        /// <remarks>
        ///     <para>
        ///         <see cref="Begin" /> must be called before enqueuing any draw calls. When all the geometry have been enqueued
        ///         for drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        protected void EnqueueDraw(ushort startIndex, ushort indexCount, ref TCommandContext itemData, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchCommandQueuer.EnqueueDraw(startIndex, indexCount, ref itemData, sortKey);
        }
    }
}
