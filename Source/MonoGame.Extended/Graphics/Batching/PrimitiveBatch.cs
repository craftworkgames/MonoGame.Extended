using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        public const int DefaultMaximumBatchSize = 8192;

        private BatchDrawer<TVertexType> _batchDrawer;
        private BatchQueuer<TVertexType> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType> _deferredBatchQueuer;  

        public BatchDrawStrategy DrawStrategy { get; }
        public bool HasBegun { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, Action<Array, Array> sortKeyValueArraysMethod, BatchDrawStrategy batchDrawStrategy = BatchDrawStrategy.UserPrimitives, int maximumBatchSize = DefaultMaximumBatchSize)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            if (sortKeyValueArraysMethod == null)
            {
                throw new ArgumentOutOfRangeException(nameof(sortKeyValueArraysMethod));
            }

            GraphicsDevice = graphicsDevice;

            DrawStrategy = batchDrawStrategy;
            switch (batchDrawStrategy)
            {
                case BatchDrawStrategy.UserPrimitives:
                {
                    _batchDrawer = new UserPrimitivesBatchDrawer<TVertexType>(graphicsDevice, maximumBatchSize);
                    break;
                }
                case BatchDrawStrategy.DynamicVertexBuffer:
                {
                    _batchDrawer = new DynamicVertexBufferBatchDrawer<TVertexType>(graphicsDevice, maximumBatchSize);
                    break;
                }
            }

            _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType>(_batchDrawer);
            _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType>(_batchDrawer, sortKeyValueArraysMethod);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            _batchDrawer.Dispose();
            _batchDrawer = null;

            _currentBatchQueuer = null;
            _immediateBatchQueuer.Dispose();
            _immediateBatchQueuer = null;
            _deferredBatchQueuer.Dispose();
            _deferredBatchQueuer = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureHasBegun([CallerMemberName] string callerMemberName = null)
        {
            if (!HasBegun)
            {
                throw new InvalidOperationException($"The {nameof(Begin)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureHasNotBegun([CallerMemberName] string callerMemberName = null)
        {
            if (HasBegun)
            {
                throw new InvalidOperationException($"The {nameof(End)} method must be called before the {callerMemberName} method can be called.");
            }
        }

        public void Begin(BatchSortMode sortMode, IDrawContext drawContext)
        {
            if (drawContext == null)
            {
                throw new ArgumentNullException(nameof(drawContext));
            }

            EnsureHasNotBegun();
            HasBegun = true;
            switch (sortMode)
            {
                case BatchSortMode.Immediate:
                    _currentBatchQueuer = _immediateBatchQueuer;
                    break;
                case BatchSortMode.Deferred:
                    _currentBatchQueuer = _deferredBatchQueuer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortMode));
            }
            _currentBatchQueuer.Begin(drawContext);
        }

        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;
            _currentBatchQueuer.End();
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, sortKey);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, sortKey);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, sortKey);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
        }
    }
}
