using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        public const int DefaultMaximumBatchVerticesSizeKiloBytes = 1 * 1024;
        public const int DefaultMaximumBatchIndicesSizeKiloBytes = 3 * 1024;

        private BatchDrawer<TVertexType> _batchDrawer;
        private BatchQueuer<TVertexType> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType> _deferredBatchQueuer;  

        public BatchDrawStrategy DrawStrategy { get; }
        public bool HasBegun { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, BatchDrawStrategy batchDrawStrategy = BatchDrawStrategy.UserPrimitives, int maximumBatchVerticesSizeKiloBytes = DefaultMaximumBatchVerticesSizeKiloBytes, int maximumBatchIndicesSizeKiloBytes = DefaultMaximumBatchIndicesSizeKiloBytes)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            GraphicsDevice = graphicsDevice;

            DrawStrategy = batchDrawStrategy;
            switch (batchDrawStrategy)
            {
                case BatchDrawStrategy.UserPrimitives:
                {
                    _batchDrawer = new UserPrimitivesBatchDrawer<TVertexType>(graphicsDevice, maximumBatchVerticesSizeKiloBytes, maximumBatchIndicesSizeKiloBytes);
                    break;
                }
                case BatchDrawStrategy.DynamicVertexBuffer:
                {
                    _batchDrawer = new DynamicVertexBufferBatchDrawer<TVertexType>(graphicsDevice, maximumBatchVerticesSizeKiloBytes, maximumBatchIndicesSizeKiloBytes);
                    break;
                }
            }

            _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType>(_batchDrawer);
            _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType>(_batchDrawer);
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

        public void Begin(BatchSortMode sortMode, Effect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
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
            _currentBatchQueuer.Begin();
        }

        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;
            _currentBatchQueuer.End();
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, IDrawContext drawContext)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, IDrawContext drawContext)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext)
        {
            EnsureHasBegun();
            _currentBatchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, drawContext);
        }
    }
}
