using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        public const int DefaultMaximumVerticesCount = 8192;
        public const int DefaultMaximumIndicesCount = 12288;

        private BatchDrawer<TVertexType, TBatchItemData> _batchDrawer;
        private BatchQueuer<TVertexType, TBatchItemData> _currentBatchQueuer;
        private ImmediateBatchQueuer<TVertexType, TBatchItemData> _immediateBatchQueuer;
        private DeferredBatchQueuer<TVertexType, TBatchItemData> _deferredBatchQueuer;

        protected RenderGeometryBuffer<TVertexType> GeometryBuffer { get; }

        public GraphicsDevice GraphicsDevice { get; }
        public bool HasBegun { get; private set; }
        public int MaximumVerticesCount { get; }
        public int MaximumIndicesCount { get; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice, int maximumVerticesCount = DefaultMaximumVerticesCount, int maximumIndicesCount = DefaultMaximumIndicesCount)
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

            GraphicsDevice = graphicsDevice;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;
            GeometryBuffer = new RenderGeometryBuffer<TVertexType>(maximumVerticesCount, maximumIndicesCount);
            _batchDrawer = new BatchDrawer<TVertexType, TBatchItemData>(graphicsDevice, GeometryBuffer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
            {
                return;
            }

            _batchDrawer?.Dispose();
            _batchDrawer = null;
            _immediateBatchQueuer?.Dispose();
            _immediateBatchQueuer = null;
            _deferredBatchQueuer?.Dispose();
            _deferredBatchQueuer = null;
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
                    if (_immediateBatchQueuer == null)
                    {
                        _immediateBatchQueuer = new ImmediateBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer);
                    }
                    _currentBatchQueuer = _immediateBatchQueuer;
                    break;
                case BatchMode.Deferred:
                    if (_deferredBatchQueuer == null)
                    {
                        _deferredBatchQueuer = new DeferredBatchQueuer<TVertexType, TBatchItemData>(_batchDrawer);
                    }
                    _currentBatchQueuer = _deferredBatchQueuer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            _currentBatchQueuer.Begin(effect);
        }

        public void End()
        {
            EnsureHasBegun();
            HasBegun = false;
            _currentBatchQueuer.End();
        }

//        private void X()
//        {
//            var remainingVertices = MaximumVerticesCount - _vertexCount;
//            var remainingIndices = MaximumIndicesCount - _indexCount;
//
//            var exceedsBatchSpace = (vertexCount > remainingVertices) || (indexCount > remainingIndices);
//
//            if (exceedsBatchSpace)
//            {
//                throw new Exception(message: "Deferred batch overflow. Deferred batching currently doesn't support fragmentation. Either decrease the number of vertices and or number of indices being drawn or increase the maximum number of vertices and or maximum number of indices.");
//            }
//        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, int[] indices, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();

            var geoemtryStartIndex = GeometryBuffer.IndicesCount;

            GeometryBuffer.Enqueue(vertices, startVertex, vertexCount, indices, startIndex, indexCount);

            _currentBatchQueuer.EnqueueDraw(primitiveType, vertexCount, geoemtryStartIndex, indexCount, ref data, sortKey);
        }

        public void Draw(PrimitiveType primitiveType, Action<RenderGeometryBuffer<TVertexType>> geometryAction, ref TBatchItemData data, uint sortKey = 0)
        {
            EnsureHasBegun();

            if (geometryAction == null)
            {
                return;
            }

            var geometryStartVertex = GeometryBuffer.VerticesCount;
            var geometryStartIndex = GeometryBuffer.IndicesCount;

            geometryAction(GeometryBuffer);

            var vertexCount = GeometryBuffer.VerticesCount - geometryStartVertex;
            var indexCount = GeometryBuffer.IndicesCount - geometryStartIndex;

            _currentBatchQueuer.EnqueueDraw(primitiveType, vertexCount, geometryStartIndex, indexCount, ref data, sortKey);
        }

        protected void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0)
        {
            _currentBatchQueuer.EnqueueDraw(primitiveType, vertexCount, startIndex, indexCount, ref data, sortKey);
        }
    }
}
