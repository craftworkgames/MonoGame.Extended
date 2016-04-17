using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DeferredBatchQueuer
    {
        internal static readonly uint[] EmptyDrawOperationKeys = new uint[0];
        internal static readonly BatchDrawOperation[] EmptyBatchDrawOperations = new BatchDrawOperation[0];
    }

    internal class DeferredBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private readonly Action<Array, Array> _sortKeyValueArraysMethod; 
        private uint _drawOperationsCount;
        private readonly uint[] _drawOperationKeys = DeferredBatchQueuer.EmptyDrawOperationKeys;
        private readonly BatchDrawOperation[] _drawOperations = DeferredBatchQueuer.EmptyBatchDrawOperations;
        private readonly BatchDrawOperation _emptyDrawOperation = new BatchDrawOperation((PrimitiveType)(-1), 0, 0, 0, 0, 0);
        private BatchDrawOperation _currentOperation;
        private readonly TVertexType[] _vertices;
        private int _usedVertexCount;
        private readonly short[] _indices;
        private int _usedIndexCount;
        private Effect _effect;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType> batchDrawer, Action<Array, Array> sortKeyValueArraysMethod)
            : base(batchDrawer)
        {
            Debug.Assert(sortKeyValueArraysMethod != null);
            _sortKeyValueArraysMethod = sortKeyValueArraysMethod;
            _vertices = new TVertexType[batchDrawer.MaximumBatchSize];
            _indices = new short[batchDrawer.MaximumBatchSize];
            _currentOperation = _emptyDrawOperation;
        }

        private void CreateNewOperationIfNecessary(PrimitiveType primitiveType, int vertexCount, int indexCount, uint sortkey)
        {
            var wasIndexed = _currentOperation.IndexCount > 0;
            var isIndexed = indexCount > 0;

            var currentOperationPrimitiveType = (PrimitiveType)_currentOperation.PrimitiveType;
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each list or triangle strip
            if (wasIndexed == isIndexed && primitiveType == currentOperationPrimitiveType && (primitiveType != PrimitiveType.TriangleStrip || primitiveType != PrimitiveType.LineStrip))
            {
                _currentOperation.VertexCount += vertexCount;
                _currentOperation.IndexCount += indexCount;
                return;
            }

            _currentOperation = new BatchDrawOperation(primitiveType, _usedVertexCount, vertexCount, _usedIndexCount, indexCount, sortkey);
            _drawOperations[_drawOperationsCount] = _currentOperation;
            _drawOperationKeys[_drawOperationsCount] = sortkey;
            _drawOperationsCount++;
        }

        internal override void Begin(Effect effect)
        {
            Debug.Assert(effect != null);
            _effect = effect;
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
                BatchDrawer.Begin(_effect, _vertices);
            }
            else
            {
                BatchDrawer.Begin(_effect, _vertices, _indices);
            }

            _sortKeyValueArraysMethod(_drawOperationKeys, _drawOperations);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _drawOperationsCount; ++index)
            {
                var operation = _drawOperations[index];
                var primitiveType = (PrimitiveType)operation.PrimitiveType;
                if (operation.IndexCount == 0)
                {
                    BatchDrawer.Draw(primitiveType, operation.StartVertex, operation.VertexCount);
                }
                else
                {
                    BatchDrawer.Draw(primitiveType, operation.StartVertex, operation.VertexCount, operation.StartIndex, operation.IndexCount);
                }
            }

            BatchDrawer.End();

            Array.Clear(_drawOperationKeys, 0, _drawOperationKeys.Length);
            Array.Clear(_drawOperations, 0, _drawOperations.Length);
            _currentOperation = _emptyDrawOperation;
            _usedVertexCount = 0;
            _usedIndexCount = 0;
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey)
        {
            var remainingSpace = BatchDrawer.MaximumBatchSize - Math.Max(_usedVertexCount, _usedIndexCount);

            if (vertexCount <= remainingSpace)
            {
                QueueVerticesNoOverflow(primitiveType, vertices, startVertex, vertexCount, sortKey);
            }
            else
            {
                QueueVerticesBufferSplit(primitiveType, vertices, startVertex, vertexCount, sortKey, remainingSpace);
            }
        }

        private void QueueVerticesNoOverflow(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortkey)
        {
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            CreateNewOperationIfNecessary(primitiveType, vertexCount, 0, sortkey);
            _usedVertexCount += vertexCount;
        }

        private void QueueVerticesBufferSplit(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey, int spaceLeft)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:
                {
                    if (spaceLeft < 2)
                    {
                        spaceLeft = 0;
                        // The single vertex will be added later in the code below
                        ++startVertex;
                        --vertexCount;
                    }
                    break;
                }
                case PrimitiveType.LineList:
                {
                    spaceLeft -= spaceLeft % 2;
                    break;
                }
                case PrimitiveType.TriangleStrip:
                {
                    if (spaceLeft < 4)
                    {
                        spaceLeft = 0;
                        // The two vertices will be added later in the code below
                        startVertex += 2;
                        vertexCount -= 2;
                    }
                    else
                    {
                        spaceLeft -= (spaceLeft - 1) % 2;
                    }
                    break;
                }
                case PrimitiveType.TriangleList:
                {
                    spaceLeft -= spaceLeft % 3;
                    break;
                }
            }

            if (spaceLeft > 0)
            {
                Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, spaceLeft);
                CreateNewOperationIfNecessary(primitiveType, spaceLeft, 0, sortKey);
                _usedVertexCount += spaceLeft;
                vertexCount -= spaceLeft;
                startVertex += spaceLeft;
            }

            Flush();

            while (vertexCount >= 0)
            {
                spaceLeft = BatchDrawer.MaximumBatchSize;

                switch (primitiveType)
                {
                    case PrimitiveType.LineStrip:
                    {
                        _vertices[0] = vertices[startVertex - 1];
                        --spaceLeft;
                        ++_usedVertexCount;
                        break;
                    }
                    case PrimitiveType.LineList:
                    {
                        spaceLeft -= spaceLeft % 2;
                        break;
                    }
                    case PrimitiveType.TriangleStrip:
                    {
                        _vertices[0] = vertices[startVertex - 2];
                        _vertices[1] = vertices[startVertex - 1];
                        spaceLeft -= (spaceLeft - 1) % 2 + 2;
                        _usedVertexCount += 2;

                        break;
                    }
                    case PrimitiveType.TriangleList:
                    {
                        spaceLeft -= spaceLeft % 3;
                        break;
                    }
                }

                var verticesToProcess = Math.Min(spaceLeft, vertexCount);
                Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, verticesToProcess);
                CreateNewOperationIfNecessary(primitiveType, verticesToProcess, 0, sortKey);
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

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey)
        {
            var remainingVertices = BatchDrawer.MaximumBatchSize - _usedVertexCount;
            var remainingIndices = BatchDrawer.MaximumBatchSize - _usedIndexCount;

            var exceedsBatchSpace = (vertexCount > remainingVertices) || (indexCount > remainingIndices);

            if (!exceedsBatchSpace)
            {
                QueueIndexedVerticesNoOverflow(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey);
            }
            else
            {
                var remainingSpace = Math.Min(remainingVertices, remainingIndices);
                QueueIndexedVerticesBufferSplit(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, sortKey, remainingSpace);
            }
        }

        private void QueueIndexedVerticesNoOverflow(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey)
        {
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            var indexOffset = _currentOperation.VertexCount;
            CreateNewOperationIfNecessary(primitiveType, vertexCount, indexCount, sortKey);
            _usedVertexCount += vertexCount;

            // we can't use Array.Copy to copy the indices because we need to add the offset
            var maxIndexCount = startIndex + indexCount;
            for (var index = startIndex; index < maxIndexCount; ++index)
            {
                _indices[_usedIndexCount++] = (short)(indices[index] + indexOffset);
            }
        }

        private void QueueIndexedVerticesBufferSplit(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey, int spaceLeft)
        {
            switch (type)
            {
                case PrimitiveType.LineStrip:
                {
                    if (spaceLeft < 2)
                    {
                        spaceLeft = 0;
                        ++startIndex;
                        --indexCount;
                    }
                    break;
                }
                case PrimitiveType.LineList:
                {
                    spaceLeft -= spaceLeft % 2;
                    break;
                }
                case PrimitiveType.TriangleStrip:
                {
                    if (spaceLeft < 4)
                    {
                        spaceLeft = 0;
                        startIndex += 2;
                        indexCount -= 2;
                    }
                    else
                    {
                        spaceLeft -= (spaceLeft - 1) % 2;
                    }
                    break;
                }
                case PrimitiveType.TriangleList:
                {
                    spaceLeft -= spaceLeft % 3;
                    break;
                }
            }

            if (spaceLeft > 0)
            {
                // vertices and indices are bijective to fill in the remaining space for this batch
                CreateNewOperationIfNecessary(type, spaceLeft, spaceLeft, sortKey);
                var maxVertexCount = _usedVertexCount + spaceLeft;
                for (var vertexIndex = startVertex; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    _indices[_usedIndexCount++] = (short)vertexIndex;
                    _vertices[_usedVertexCount++] = vertices[indices[startIndex] + startVertex];
                    ++startIndex;
                }
                indexCount -= spaceLeft;
            }

            Flush();

            while (indexCount >= 0 && vertexCount >= 0)
            {
                spaceLeft = BatchDrawer.MaximumBatchSize;

                switch (type)
                {
                    case PrimitiveType.LineStrip:
                    {
                        _vertices[0] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[0] = 0;

                        --spaceLeft;
                        ++_usedVertexCount;
                        ++_usedIndexCount;
                        break;
                    }
                    case PrimitiveType.LineList:
                    {
                        spaceLeft -= spaceLeft % 2;
                        break;
                    }
                    case PrimitiveType.TriangleStrip:
                    {
                        _vertices[0] = vertices[indices[startIndex - 2] + startVertex];
                        _indices[0] = 0;
                        _vertices[1] = vertices[indices[startIndex - 1] + startVertex];
                        _indices[1] = 1;

                        spaceLeft -= (spaceLeft - 1) % 2 + 2;
                        _usedIndexCount += 2;
                        _usedVertexCount += 2;
                        break;
                    }
                    case PrimitiveType.TriangleList:
                    {
                        spaceLeft -= spaceLeft % 3;
                        break;
                    }
                }

                var verticesToProcess = 0;
                var indicesToProcess = Math.Min(spaceLeft, indexCount);

                var maxVertexCount = _usedVertexCount + indicesToProcess;
                for (var vertexIndex = _usedVertexCount; vertexIndex < maxVertexCount; ++vertexIndex)
                {
                    // if vertex is already been seen, use that index
                    // if vertex has not already been seem, create the index and copy the vertex
                    _indices[_usedIndexCount++] = (short)vertexIndex;
                    _vertices[_usedVertexCount++] = vertices[indices[startIndex] + startVertex];
                    ++startIndex;
                }

                CreateNewOperationIfNecessary(type, verticesToProcess, indicesToProcess, sortKey);

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
