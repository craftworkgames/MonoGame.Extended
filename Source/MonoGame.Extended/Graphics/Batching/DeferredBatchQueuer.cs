using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DeferredBatchQueuer
    {
        // prevent static fields for every generic type of the class

        // an empty draw operation; it's purpose is to prevent a null check for what would be a nullable draw operation
        // e.g. the first time the queuer is used the draw operation would be invalid, but afterwards the operation will always be valid,
        //      so having a check for null to test for invalidness is a waste 99% of the time
        internal static readonly BatchDrawOperation EmptyDrawOperation = new BatchDrawOperation((PrimitiveType)(-1), 0, 0, 0, 0, null);
    }

    internal class DeferredBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private const int InitialOperationsCapacity = 25;

        // the draw operations buffer
        private BatchDrawOperation[] _operations;
        // the sort keys buffer for the draw operations
        // the keys are seperated from the draw operations to have the smallest memory footprint possible which is important for iteration
        private uint[] _operationSortKeys; 
        // the number of operations in the buffers
        private int _operationsCount;
        // the current draw operation
        private BatchDrawOperation _currentOperation;
        // the sort key for the current draw operation
        private uint _currentSortKey;
        // the vertices buffer
        private readonly TVertexType[] _vertices;
        // the number of vertices in the buffer
        private int _vertexCount;
        // the index buffer
        private readonly short[] _indices;
        // the number of indices in the buffer
        private int _indexCount;
        // the sort method (inlined) to sort the keys for the draw operations; when sorted the corresponding draw operations are also sorted
        private readonly Action<Array, Array, int, int> _sortKeysValuesAction;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType> batchDrawer, Action<Array, Array, int, int> sortKeysValuesAction)
            : base(batchDrawer)
        {
            _currentOperation = DeferredBatchQueuer.EmptyDrawOperation;
            _vertices = new TVertexType[BatchDrawer.MaximumVerticesCount];
            _indices = new short[BatchDrawer.MaximumIndicesCount];
            _sortKeysValuesAction = sortKeysValuesAction;
            _operationSortKeys = new uint[InitialOperationsCapacity];
            _operations = new BatchDrawOperation[InitialOperationsCapacity];
        }

        private void CreateNewDrawOperationIfNecessary(IDrawContext drawContext, PrimitiveType primitiveType, int vertexCount, int indexCount, uint sortKey)
        {
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each line strip or triangle strip
            if (_currentSortKey == sortKey && (_currentOperation.DrawContext?.Equals(drawContext) ?? false) && _currentOperation.IndexCount > 0 == indexCount > 0 && _currentOperation.PrimitiveType == (byte)primitiveType && (primitiveType != PrimitiveType.TriangleStrip || PrimitiveType != PrimitiveType.LineStrip))
            {
                _currentOperation.VertexCount += vertexCount;
                _currentOperation.IndexCount += indexCount;
                return;
            }

            _currentOperation.DrawContext = drawContext;
            _currentOperation = new BatchDrawOperation(primitiveType, _vertexCount, vertexCount, _indexCount, indexCount, _currentOperation.DrawContext);
            AddOperation(_currentOperation, sortKey);
        }

        private void AddOperation(BatchDrawOperation batchOperation, uint sortKey)
        {
            if (_operationsCount >= _operations.Length)
            {
                // increase draw operation buffers by the golden ratio
                var newCapacity = (int)(_operations.Length * 1.61803398875f);
                Array.Resize(ref _operationSortKeys, newCapacity);
                Array.Resize(ref _operations, newCapacity);
            }
            _operationSortKeys[_operationsCount] = sortKey;
            _operations[_operationsCount] = batchOperation;
            ++_operationsCount;
            _currentSortKey = sortKey;
        }

        internal override void Begin()
        {
        }

        internal override void End()
        {
            Flush();
        }

        private void Flush()
        {
            if (_vertexCount == 0)
            {
                return;
            }

            if (_indexCount == 0)
            {
                BatchDrawer.Select(_vertices);
            }
            else
            {
                BatchDrawer.Select(_vertices, _indices);
            }

            // sort only the operations which are used
            _sortKeysValuesAction(_operationSortKeys, _operations, 0, _operationsCount);

            for (var index = 0; index < _operationsCount; index++)
            {
                var operation = _operations[index];
                var primitiveType = (PrimitiveType)operation.PrimitiveType;
                if (operation.IndexCount != 0)
                {
                    BatchDrawer.Draw(operation.DrawContext, primitiveType, operation.StartVertex, operation.VertexCount, operation.StartIndex, operation.IndexCount);
                }
                else
                {
                    BatchDrawer.Draw(operation.DrawContext, primitiveType, operation.StartVertex, operation.VertexCount);
                }
            }

            Array.Clear(_operations, 0, _operationsCount);
            _operationsCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
            _currentOperation = DeferredBatchQueuer.EmptyDrawOperation;
            _currentSortKey = 0;
        }

        internal override void EnqueueDraw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _vertexCount;

            if (remainingVertices >= vertexCount)
            {
                QueueVerticesNoOverflow(drawContext, primitiveType, vertices, startVertex, vertexCount, sortKey);
            }
            else
            {
                QueueVerticesBufferSplit(drawContext, primitiveType, vertices, startVertex, vertexCount, sortKey, remainingVertices);
            }
        }

        private void QueueVerticesNoOverflow(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey)
        {
            Array.Copy(vertices, startVertex, _vertices, _vertexCount, vertexCount);
            CreateNewDrawOperationIfNecessary(drawContext, primitiveType, vertexCount, 0, sortKey);
            _vertexCount += vertexCount;
        }

        private void QueueVerticesBufferSplit(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey, int verticesSpaceLeft)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.LineStrip:
                    if (verticesSpaceLeft < 2)
                    {
                        verticesSpaceLeft = 0;
                        // The vertex will be added later in the code below
                        ++startVertex;
                        --vertexCount;
                    }
                    break;
                case PrimitiveType.LineList:
                    verticesSpaceLeft -= verticesSpaceLeft % 2;
                    break;
                case PrimitiveType.TriangleStrip:
                    if (verticesSpaceLeft < 4)
                    {
                        verticesSpaceLeft = 0;
                        // The two vertices will be added later in the code below
                        startVertex += 2;
                        vertexCount -= 2;
                    }
                    else
                    {
                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2;
                    }
                    break;
                case PrimitiveType.TriangleList:
                    verticesSpaceLeft -= verticesSpaceLeft % 3;
                    break;
            }

            if (verticesSpaceLeft > 0)
            {
                Array.Copy(vertices, startVertex, _vertices, _vertexCount, verticesSpaceLeft);
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesSpaceLeft, 0, sortKey);
                _vertexCount += verticesSpaceLeft;
                vertexCount -= verticesSpaceLeft;
                startVertex += verticesSpaceLeft;
            }

            Flush();

            while (vertexCount >= 0)
            {
                verticesSpaceLeft = BatchDrawer.MaximumVerticesCount;

                switch (PrimitiveType)
                {
                    case PrimitiveType.LineStrip:
                        _vertices[0] = vertices[startVertex - 1];
                        --verticesSpaceLeft;
                        ++_vertexCount;
                        break;
                    case PrimitiveType.LineList:
                        verticesSpaceLeft -= verticesSpaceLeft % 2;
                        break;
                    case PrimitiveType.TriangleStrip:
                        _vertices[0] = vertices[startVertex - 2];
                        _vertices[1] = vertices[startVertex - 1];
                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2 + 2;
                        _vertexCount += 2;
                        break;
                    case PrimitiveType.TriangleList:
                        verticesSpaceLeft -= verticesSpaceLeft % 3;
                        break;
                }

                var verticesToProcess = Math.Min(verticesSpaceLeft, vertexCount);
                Array.Copy(vertices, startVertex, _vertices, _vertexCount, verticesToProcess);
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesToProcess, 0, sortKey);
                _vertexCount += verticesToProcess;
                vertexCount -= verticesToProcess;

                if (vertexCount == 0)
                {
                    break;
                }

                Flush();
                startVertex += verticesToProcess;
            }
        }

        internal override void EnqueueDraw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _vertexCount;
            var remainingIndices = BatchDrawer.MaximumIndicesCount - _indexCount;

            var exceedsBatchSpace = (vertexCount > remainingVertices) || (indexCount > remainingIndices);

            if (!exceedsBatchSpace)
            {
                QueueIndexedVerticesNoOverflow(drawContext, primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
            }
            else
            {
                QueueIndexedVerticesBufferSplit(drawContext, primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey, remainingVertices, remainingIndices);
            }
        }

        private void QueueIndexedVerticesNoOverflow(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey)
        {
            Array.Copy(vertices, startVertex, _vertices, _vertexCount, vertexCount);
            var indexOffset = _currentOperation.VertexCount;
            CreateNewDrawOperationIfNecessary(drawContext, primitiveType, vertexCount, indexCount, sortKey);
            _vertexCount += vertexCount;

            // we can't use Array.Copy to copy the indices because we need to add the offset
            var maxIndexCount = startIndex + indexCount;
            for (var index = startIndex; index < maxIndexCount; ++index)
            {
                _indices[_indexCount++] = (short)(indices[index] + indexOffset);
            }
        }

        private void QueueIndexedVerticesBufferSplit(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey, int verticesSpaceLeft, int indicesSpaceLeft)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.LineStrip:
                    if (verticesSpaceLeft < 2)
                    {
                        verticesSpaceLeft = 0;
                        ++startIndex;
                        --indexCount;
                    }
                    break;
                case PrimitiveType.LineList:
                    verticesSpaceLeft -= verticesSpaceLeft % 2;
                    break;
                case PrimitiveType.TriangleStrip:
                    if (verticesSpaceLeft < 4)
                    {
                        verticesSpaceLeft = 0;
                        startIndex += 2;
                        indexCount -= 2;
                    }
                    else
                    {
                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2;
                    }
                    break;
                case PrimitiveType.TriangleList:
                    verticesSpaceLeft -= verticesSpaceLeft % 3;
                    break;
            }

            if (verticesSpaceLeft > 0)
            {
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesSpaceLeft, verticesSpaceLeft, sortKey);
                var maxVertexCount = _vertexCount + verticesSpaceLeft;
                for (var vertexIndex = startVertex; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    _indices[_indexCount++] = (short)vertexIndex;
                    _vertices[_vertexCount++] = vertices[indices[startIndex] + startVertex];
                    ++startIndex;
                }
                indexCount -= verticesSpaceLeft;
            }

            Flush();

            while (indexCount >= 0 && vertexCount >= 0)
            {
                verticesSpaceLeft = BatchDrawer.MaximumVerticesCount;
                indicesSpaceLeft = BatchDrawer.MaximumIndicesCount;

                switch (PrimitiveType)
                {
                    case PrimitiveType.LineStrip:
                        _vertices[0] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[0] = 0;

                        --verticesSpaceLeft;
                        ++_vertexCount;
                        ++_indexCount;
                        break;
                    case PrimitiveType.LineList:
                        verticesSpaceLeft -= verticesSpaceLeft % 2;
                        break;
                    case PrimitiveType.TriangleStrip:
                        _vertices[0] = vertices[indices[startIndex - 2] + startVertex];
                        _indices[0] = 0;
                        _vertices[1] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[1] = 1;

                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2 + 2;
                        _indexCount += 2;
                        _vertexCount += 2;
                        break;
                    case PrimitiveType.TriangleList:
                        verticesSpaceLeft -= verticesSpaceLeft % 3;
                        break;
                }

                var verticesToProcess = 0;
                var indicesToProcess = Math.Min(verticesSpaceLeft, indexCount);

                var maxVertexCount = _vertexCount + indicesToProcess;
                for (var vertexIndex = _vertexCount; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    // if vertex is already been seen, use that index
                    // if vertex has not already been seem, create the index and copy the vertex
                    _indices[_indexCount++] = (short)vertexIndex;
                    _vertices[_vertexCount++] = vertices[indices[startIndex] + startVertex];
                    ++startIndex;
                }

                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesToProcess, indicesToProcess, sortKey);

                indexCount -= indicesToProcess;
                if (indexCount == 0)
                {
                    break;
                }

                Flush();
            }
        }
    }
}
