using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        // for 2D games I would suspect most people would use VertexPositionColorTexture for sprites like SpriteBatch
        // SpriteBatch uses two triangles to create a rectangle using triangle list; 4 vertices and 6 indices
        // the size is 24 bytes per vertex and 2 bytes per index
        // XNA's SpriteBatch: 
        //      -uses up to 2048 sprites per batch, so up to 8192 vertices and 12288 indices per batch
        //      -uses DynamicVertexBuffer
        // FNA's SpriteBatch:
        //      -uses up to 2048 sprites per batch, so up to 8192 vertices and 12288 indices per batch
        //      -uses DynamicVertexBuffer
        // MonoGame's SpriteBatch:
        //      -uses 256 sprites per batch initially, so 1024 vertices and 1536 indices per batch, but
        //      dynamically increases the size of the batch by x1.5 when the batch is full (it doesn't appear to have a max limit?!?)
        //      -uses UserPrimitives
        //
        // It appears that 2048 sprites ber batch is the expected default.
        //
        // vertices: 
        //      8192 VertexPositionColor vertices * 24/1 bytes per vertex = 196608 bytes
        //      196608 bytes * 1/1024 kibibytes per byte = 192 kibibytes
        // indices:
        //      12288 indices * 2/1 bytes per index = 24576 bytes
        //      24576 bytes * 1/1024 kibibytes per byte = 24 kibibytes

        public const ushort DefaultMaximumVerticesCount = 8192;
        public const ushort DefaultMaximumIndicesCount = 12288;

        private BatchDrawer<TVertexType> _batchDrawer;
        private BatchQueuer<TVertexType> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType> _deferredBatchQueuer;  

        public BatchDrawStrategy DrawStrategy { get; }
        public bool HasBegun { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, Action<Array, Array> sortKeysValuesAction, BatchDrawStrategy batchDrawStrategy = BatchDrawStrategy.UserPrimitives, ushort maximumVerticesCount = DefaultMaximumVerticesCount, ushort maximumIndicesCount = DefaultMaximumIndicesCount)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }

            if (sortKeysValuesAction == null)
            {
                throw new ArgumentNullException(nameof(sortKeysValuesAction));
            }

            if (batchDrawStrategy > BatchDrawStrategy.DynamicVertexBuffer)
            {
                throw new ArgumentOutOfRangeException(nameof(batchDrawStrategy));
            }

            if (maximumVerticesCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumVerticesCount));
            }

            if (maximumIndicesCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumIndicesCount));
            }

            GraphicsDevice = graphicsDevice;

            DrawStrategy = batchDrawStrategy;
            switch (batchDrawStrategy)
            {
                case BatchDrawStrategy.UserPrimitives:
                {
                    _batchDrawer = new UserPrimitivesBatchDrawer<TVertexType>(graphicsDevice, maximumVerticesCount, maximumIndicesCount);
                    break;
                }
                case BatchDrawStrategy.DynamicVertexBuffer:
                {
                    _batchDrawer = new DynamicVertexBufferBatchDrawer<TVertexType>(graphicsDevice, maximumVerticesCount, maximumIndicesCount);
                    break;
                }
            }

            _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType>(_batchDrawer);
            _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType>(_batchDrawer, sortKeysValuesAction);
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

        public void Begin(BatchSortMode sortMode)
        {
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

        public void Draw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, uint sortkey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(drawContext, primitiveType, vertices, 0, vertices.Length, sortkey);
        }

        public void Draw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(drawContext, primitiveType, vertices, startVertex, vertexCount, sortKey);
        }

        public void Draw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(drawContext, primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, sortKey);
        }

        public void Draw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            EnsureHasBegun();
            _currentBatchQueuer.EnqueueDraw(drawContext, primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
        }
    }
}
