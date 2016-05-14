using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DeferredBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private const int InitialOperationsCapacity = 25;

        private uint[] _operationKeys; 
        private BatchDrawOperation[] _operations;
        private int _nextFreeOperationIndex;
        private uint _currentSortKey;
        private BatchDrawOperation _currentOperation;
        private readonly BatchDrawOperation _emptyDrawOperation = new BatchDrawOperation((PrimitiveType)(-1), 0, 0, 0, 0, null);
        private readonly TVertexType[] _vertices;
        private int _usedVertexCount;
        private readonly short[] _indices;
        private int _usedIndexCount;
        private readonly Action<Array, Array> _sortKeysValuesAction;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType> batchDrawer, Action<Array, Array> sortKeysValuesAction)
            : base(batchDrawer)
        {
            _currentOperation = _emptyDrawOperation;
            _vertices = new TVertexType[BatchDrawer.MaximumVerticesCount];
            _indices = new short[BatchDrawer.MaximumIndicesCount];
            _sortKeysValuesAction = sortKeysValuesAction;
            _operationKeys = new uint[InitialOperationsCapacity];
            _operations = new BatchDrawOperation[InitialOperationsCapacity];
        }

        private void CreateNewDrawOperationIfNecessary(IDrawContext drawContext, PrimitiveType primitiveType, int vertexCount, int indexCount, uint sortKey)
        {
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each line strip or triangle strip
            if (_currentSortKey == sortKey && _currentOperation.DrawContext.Equals(drawContext) && _currentOperation.PrimitiveType == (byte)primitiveType && (primitiveType != PrimitiveType.TriangleStrip || PrimitiveType != PrimitiveType.LineStrip) && _currentOperation.IndexCount > 0 == indexCount > 0)
            {
                _currentOperation.VertexCount += vertexCount;
                _currentOperation.IndexCount += indexCount;
                return;
            }

            _currentOperation = new BatchDrawOperation(primitiveType, _usedVertexCount, vertexCount, _usedIndexCount, indexCount, _currentOperation.DrawContext);
            AddOperation(ref _currentOperation, sortKey);
        }

        private void AddOperation(ref BatchDrawOperation batchOperation, uint sortKey)
        {
            if (_nextFreeOperationIndex >= _operations.Length)
            {
                // increase array buffer sizes by the golden ratio
                var newCapacity = (int)(_operations.Length * 1.61803398875f);
                Array.Resize(ref _operationKeys, newCapacity);
                Array.Resize(ref _operations, newCapacity);
            }
            _operationKeys[_nextFreeOperationIndex] = sortKey;
            _operations[_nextFreeOperationIndex] = batchOperation;
            ++_nextFreeOperationIndex;
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
            if (_usedVertexCount == 0)
            {
                return;
            }

            if (_usedIndexCount == 0)
            {
                BatchDrawer.Select(_vertices);
            }
            else
            {
                BatchDrawer.Select(_vertices, _indices);
            }

            _sortKeysValuesAction(_operationKeys, _operations);

            // iterating an ARRAY using "foreach" is just as \fast\ as using "for"; the compiler will produce identical machine code; however, this is not true for LIST!
            // using unsafe code to iterate the array to avoid bound checks is unnecessary with "foreach"; the compiler understands accessing beyond the array is impossible
            // "foreach" is a lot simpler for us humans to read as well, yay!
            foreach (var operation in _operations)
            {
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

            Array.Clear(_operations, 0, _operations.Length);
            _currentOperation = _emptyDrawOperation;
            _usedVertexCount = 0;
            _usedIndexCount = 0;
        }

        internal override void EnqueueDraw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0)
        {
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _usedVertexCount;

            if (vertexCount <= remainingVertices)
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
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            CreateNewDrawOperationIfNecessary(drawContext, primitiveType, vertexCount, 0, sortKey);
            _usedVertexCount += vertexCount;
        }

        private void QueueVerticesBufferSplit(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey, int verticesSpaceLeft)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.LineStrip:
                {
                    if (verticesSpaceLeft < 2)
                    {
                        verticesSpaceLeft = 0;
                        // The single vertex will be added later in the code below
                        ++startVertex;
                        --vertexCount;
                    }
                    break;
                }
                case PrimitiveType.LineList:
                {
                    verticesSpaceLeft -= verticesSpaceLeft % 2;
                    break;
                }
                case PrimitiveType.TriangleStrip:
                {
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
                }
                case PrimitiveType.TriangleList:
                {
                    verticesSpaceLeft -= verticesSpaceLeft % 3;
                    break;
                }
            }

            if (verticesSpaceLeft > 0)
            {
                Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, verticesSpaceLeft);
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesSpaceLeft, 0, sortKey);
                _usedVertexCount += verticesSpaceLeft;
                vertexCount -= verticesSpaceLeft;
                startVertex += verticesSpaceLeft;
            }

            Flush();

            while (vertexCount >= 0)
            {
                //verticesSpaceLeft = BatchDrawer.MaximumBatchSizeKiloBytes;

                switch (PrimitiveType)
                {
                    case PrimitiveType.LineStrip:
                    {
                        _vertices[0] = vertices[startVertex - 1];
                        --verticesSpaceLeft;
                        ++_usedVertexCount;
                        break;
                    }
                    case PrimitiveType.LineList:
                    {
                        verticesSpaceLeft -= verticesSpaceLeft % 2;
                        break;
                    }
                    case PrimitiveType.TriangleStrip:
                    {
                        _vertices[0] = vertices[startVertex - 2];
                        _vertices[1] = vertices[startVertex - 1];
                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2 + 2;
                        _usedVertexCount += 2;

                        break;
                    }
                    case PrimitiveType.TriangleList:
                    {
                        verticesSpaceLeft -= verticesSpaceLeft % 3;
                        break;
                    }
                }

                var verticesToProcess = Math.Min(verticesSpaceLeft, vertexCount);
                Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, verticesToProcess);
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesToProcess, 0, sortKey);
                _usedVertexCount += verticesToProcess;
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
            var remainingVertices = BatchDrawer.MaximumVerticesCount - _usedVertexCount;
            var remainingIndices = BatchDrawer.MaximumIndicesCount - _usedIndexCount;

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
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            var indexOffset = _currentOperation.VertexCount;
            CreateNewDrawOperationIfNecessary(drawContext, primitiveType, vertexCount, indexCount, sortKey);
            _usedVertexCount += vertexCount;

            // we can't use Array.Copy to copy the indices because we need to add the offset
            var maxIndexCount = startIndex + indexCount;
            for (var index = startIndex; index < maxIndexCount; ++index)
            {
                _indices[_usedIndexCount++] = (short)(indices[index] + indexOffset);
            }
        }

        private void QueueIndexedVerticesBufferSplit(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey, int verticesSpaceLeft, int indicesSpaceLeft)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.LineStrip:
                {
                    if (verticesSpaceLeft < 2)
                    {
                        verticesSpaceLeft = 0;
                        ++startIndex;
                        --indexCount;
                    }
                    break;
                }
                case PrimitiveType.LineList:
                {
                    verticesSpaceLeft -= verticesSpaceLeft % 2;
                    break;
                }
                case PrimitiveType.TriangleStrip:
                {
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
                }
                case PrimitiveType.TriangleList:
                {
                    verticesSpaceLeft -= verticesSpaceLeft % 3;
                    break;
                }
            }

            if (verticesSpaceLeft > 0)
            {
                CreateNewDrawOperationIfNecessary(drawContext, primitiveType, verticesSpaceLeft, verticesSpaceLeft, sortKey);
                var maxVertexCount = _usedVertexCount + verticesSpaceLeft;
                for (var vertexIndex = startVertex; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    _indices[_usedIndexCount++] = (short)vertexIndex;
                    _vertices[_usedVertexCount++] = vertices[indices[startIndex] + startVertex];
                    ++startIndex;
                }
                indexCount -= verticesSpaceLeft;
            }

            Flush();

            while (indexCount >= 0 && vertexCount >= 0)
            {
                //verticesSpaceLeft = BatchDrawer.MaximumBatchSizeKiloBytes;

                switch (PrimitiveType)
                {
                    case PrimitiveType.LineStrip:
                    {
                        _vertices[0] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[0] = 0;

                        --verticesSpaceLeft;
                        ++_usedVertexCount;
                        ++_usedIndexCount;
                        break;
                    }
                    case PrimitiveType.LineList:
                    {
                        verticesSpaceLeft -= verticesSpaceLeft % 2;
                        break;
                    }
                    case PrimitiveType.TriangleStrip:
                    {
                        _vertices[0] = vertices[indices[startIndex - 2] + startVertex];
                        _indices[0] = 0;
                        _vertices[1] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[1] = 1;

                        verticesSpaceLeft -= (verticesSpaceLeft - 1) % 2 + 2;
                        _usedIndexCount += 2;
                        _usedVertexCount += 2;
                        break;
                    }
                    case PrimitiveType.TriangleList:
                    {
                        verticesSpaceLeft -= verticesSpaceLeft % 3;
                        break;
                    }
                }

                var verticesToProcess = 0;
                var indicesToProcess = Math.Min(verticesSpaceLeft, indexCount);

                var maxVertexCount = _usedVertexCount + indicesToProcess;
                for (var vertexIndex = _usedVertexCount; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    // if vertex is already been seen, use that index
                    // if vertex has not already been seem, create the index and copy the vertex
                    _indices[_usedIndexCount++] = (short)vertexIndex;
                    _vertices[_usedVertexCount++] = vertices[indices[startIndex] + startVertex];
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
