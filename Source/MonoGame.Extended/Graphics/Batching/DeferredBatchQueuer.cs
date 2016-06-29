using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DeferredBatchQueuer<TVertexType, TBatchItemData, TEffect> : BatchQueuer<TVertexType, TBatchItemData, TEffect>
        where TVertexType : struct, IVertexType
        where TBatchItemData : struct, IBatchItemData<TBatchItemData, TEffect>
        where TEffect : Effect
    {
        internal static readonly BatchDrawOperation<TBatchItemData, TEffect> EmptyDrawOperation = new BatchDrawOperation<TBatchItemData, TEffect>((PrimitiveType)(-1), 0, 0, 0, 0, default(TBatchItemData));

        private const int InitialOperationsCapacity = 25;

        // the draw operations buffer
        private BatchDrawOperation<TBatchItemData, TEffect>[] _operations;
        // the sort keys buffer for the draw operations
        // the keys are seperated from the draw operations to have the smallest memory footprint possible for each draw operation
        private uint[] _operationSortKeys; 
        // the number of operations in the buffers
        private int _operationsCount;
        // the current draw operation
        private BatchDrawOperation<TBatchItemData, TEffect> _currentOperation;
        // the sort key for the current draw operation
        private uint _currentSortKey;
        // the vertices buffer
        private readonly TVertexType[] _vertices;
        // the number of vertices in the buffer
        private int _vertexCount;
        // the index buffer
        private readonly int[] _indices;
        // the number of indices in the buffer
        private int _indexCount;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType, TBatchItemData, TEffect> batchDrawer)
            : base(batchDrawer)
        {
            _currentOperation = EmptyDrawOperation;
            _vertices = new TVertexType[BatchDrawer.MaximumVerticesCount];
            _indices = new int[BatchDrawer.MaximumIndicesCount];
            _operationSortKeys = new uint[InitialOperationsCapacity];
            _operations = new BatchDrawOperation<TBatchItemData, TEffect>[InitialOperationsCapacity];
        }

        private void CreateNewDrawOperationIfNecessary(ref TBatchItemData data, PrimitiveType primitiveType, int vertexCount, int indexCount, uint sortKey)
        {
            // "merge" multiple draw calls into one draw operation if possible
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each line strip or triangle strip

            if (_currentSortKey == sortKey && _currentOperation.CanMergeIntoOperationOf(ref data, (byte)primitiveType, indexCount))
            {
                _currentOperation.VertexCount += vertexCount;
                _currentOperation.IndexCount += indexCount;
                return;
            }

            if (_operationsCount > 0)
            {
                ApplyCurrentOperation();
            }

            _currentOperation = new BatchDrawOperation<TBatchItemData, TEffect>(primitiveType, _vertexCount, vertexCount, _indexCount, indexCount, data);

            if (_operationsCount == _operations.Length)
            {
                // increase draw operation buffers by the golden ratio
                var newCapacity = (int)(_operations.Length * 1.61803398875f);
                Array.Resize(ref _operationSortKeys, newCapacity);
                Array.Resize(ref _operations, newCapacity);
            }

            _operationSortKeys[_operationsCount] = sortKey;
            _operations[_operationsCount++] = _currentOperation;
            _currentSortKey = sortKey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyCurrentOperation()
        {
            _operations[_operationsCount - 1] = _currentOperation;
        }

        internal override void Begin(TEffect effect)
        {
            BatchDrawer.Effect = effect;
        }

        internal override void End()
        {
            Flush();
            BatchDrawer.Effect = null;
        }

        private void Flush()
        {
            if (_vertexCount == 0)
            {
                return;
            }

            if (_indexCount == 0)
            {
                BatchDrawer.Select(_vertices, 0, _vertexCount);
            }
            else
            {
                BatchDrawer.Select(_vertices, 0, _vertexCount, _indices, 0, _indexCount);
            }

            ApplyCurrentOperation();

            // sort only the operations which are used
            PrimitiveBatchHelper.SortAction?.Invoke(_operationSortKeys, _operations, 0, _operationsCount);

            for (var index = 0; index < _operationsCount; index++)
            {
                var operation = _operations[index];
                var primitiveType = (PrimitiveType)operation.PrimitiveType;
                if (operation.IndexCount != 0)
                {
                    BatchDrawer.Draw(ref operation.Data, primitiveType, operation.StartVertex, operation.VertexCount, operation.StartIndex, operation.IndexCount);
                }
                else
                {
                    BatchDrawer.Draw(ref operation.Data, primitiveType, operation.StartVertex, operation.VertexCount);
                }
            }

            // don't need to clear the array because we keep track of how many operations we have
#if DEBUG
            Array.Clear(_operations, 0, _operationsCount);
#endif

            _operationsCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
            _currentOperation = EmptyDrawOperation;
            _currentSortKey = 0;
        }

        internal override void EnqueueDraw(ref TBatchItemData data, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _vertexCount;

            var exceedsBatchSpace = vertexCount > remainingVertices;

            if (exceedsBatchSpace)
            {
                throw new Exception(message: "Deferred batch overflow. Deferred batching currently doesn't support fragmentation. Either decrease the number of vertices and or number of indices being drawn or increase the maximum number of vertices and or maximum number of indices.");
            }

            CreateNewDrawOperationIfNecessary(ref data, primitiveType, vertexCount, 0, sortKey);
            Array.Copy(vertices, startVertex, _vertices, _vertexCount, vertexCount);
            _vertexCount += vertexCount;
        }

        internal override void EnqueueDraw(ref TBatchItemData data, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, int[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _vertexCount;
            var remainingIndices = BatchDrawer.MaximumIndicesCount - _indexCount;

            var exceedsBatchSpace = (vertexCount > remainingVertices) || (indexCount > remainingIndices);

            if (exceedsBatchSpace)
            {
                throw new Exception(message: "Deferred batch overflow. Deferred batching currently doesn't support fragmentation. Either decrease the number of vertices and or number of indices being drawn or increase the maximum number of vertices and or maximum number of indices.");
            }

            CreateNewDrawOperationIfNecessary(ref data, primitiveType, vertexCount, indexCount, sortKey);

            Array.Copy(vertices, startVertex, _vertices, _vertexCount, vertexCount);
            _vertexCount += vertexCount;

            Array.Copy(indices, startIndex, _indices, _indexCount, indexCount);
            var maxIndexCount = _indexCount + indexCount;
            var indexOffset = _currentOperation.VertexCount;
            while (_indexCount < maxIndexCount)
            {
                _indices[_indexCount++] += indexOffset;
            }
        }
    }
}
