using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching.Queuers;

namespace MonoGame.Extended.Graphics.Batching
{
    /// <summary>
    ///     Enables a group of geometry to drawn using the same settings.
    /// </summary>
    /// <typeparam name="TVertexType">The type of vertex.</typeparam>
    /// <typeparam name="TBatchItemData">The type of item data.</typeparam>
    /// <seealso cref="IDisposable" />
    public abstract class PrimitiveBatch<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IDrawContext<TBatchItemData>
    {
        private BatchDrawer<TVertexType, TBatchItemData> _batchDrawer;
        private BatchQueuer<TVertexType, TBatchItemData> _currentBatchQueuer;
        private Lazy<ImmediateBatchQueuer<TVertexType, TBatchItemData>> _lazyImmediateBatchQueuer;
        private Lazy<DeferredBatchQueuer<TVertexType, TBatchItemData>> _lazyDeferredBatchQueuer;

        /// <summary>
        ///     Gets the <see cref="GeometryBuilder{TVertexType}" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TBatchItemData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GeometryBuilder{TVertexType}" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TBatchItemData}" />.
        /// </value>
        protected GeometryBuffer<TVertexType> GeometryBuffer { get; }

        /// <summary>
        ///     Gets the <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TBatchItemData}" />.
        /// </summary>
        /// <value>
        ///     The <see cref="GraphicsDevice" /> associated with this
        ///     <see cref="PrimitiveBatch{TVertexType,TBatchItemData}" />.
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
        ///     Initializes a new instance of the <see cref="PrimitiveBatch{TVertexType, TBatchItemData}" /> class.
        /// </summary>
        /// <param name="geometryBuffer">The geometry buffer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="geometryBuffer" /> is null.</exception>
        protected PrimitiveBatch(GeometryBuffer<TVertexType> geometryBuffer)
        {
            if (geometryBuffer == null)
            {
                throw new ArgumentNullException(nameof(geometryBuffer));
            }

            GeometryBuffer = geometryBuffer;
            GraphicsDevice = geometryBuffer.GraphicsDevice;
            _batchDrawer = new BatchDrawer<TVertexType, TBatchItemData>(GraphicsDevice, GeometryBuffer);

            _lazyImmediateBatchQueuer = new Lazy<ImmediateBatchQueuer<TVertexType, TBatchItemData>>(() => new ImmediateBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer));
            _lazyDeferredBatchQueuer = new Lazy<DeferredBatchQueuer<TVertexType, TBatchItemData>>(() => new DeferredBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer));
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
            // ReSharper disable once InvertIf
            if (diposing)
            {
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
            {
                throw new InvalidOperationException($"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
            {
                throw new InvalidOperationException($"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        /// <summary>
        ///     Starts a group of geometry for rendering with the specified <see cref="Effect" />.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="effect">The effect.</param>
        /// <exception cref="ArgumentNullException"><paramref name="effect" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode" /> is not a valid <see cref="BatchMode" /> value.</exception>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Begin" /> cannot be invoked again until <see cref="End" /> has been invoked.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         This method must be called before any enqueuing of draw calls. When all the geometry have been enqueued for
        ///         drawing, call <see cref="End" />.
        ///     </para>
        /// </remarks>
        public void Begin(BatchMode mode, Effect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            EnsureHasNotBegun();
            HasBegun = true;

            switch (mode)
            {
                case BatchMode.Immediate:
                    _currentBatchQueuer = _lazyImmediateBatchQueuer.Value;
                    break;
                case BatchMode.Deferred:
                    _currentBatchQueuer = _lazyDeferredBatchQueuer.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            _currentBatchQueuer.Begin(effect);
        }

        /// <summary>
        ///     Submits the group of geometry to the <see cref="GraphicsDevice" /> for rendering.
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

            _currentBatchQueuer.End();
        }

        /// <summary>
        ///     Adds geometry to the group of geometry for rendering using the specified primitive type, vertices, indices, data,
        ///     and optional sort key.
        /// </summary>
        /// <param name="primitiveType">The type of the primitive.</param>
        /// <param name="vertexCount">The number of vertices from the <see cref="GeometryBuffer" /> to use.</param>
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
        protected void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TBatchItemData itemData, uint sortKey = 0)
        {
            _currentBatchQueuer.EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref itemData, sortKey);
        }
    }
}
