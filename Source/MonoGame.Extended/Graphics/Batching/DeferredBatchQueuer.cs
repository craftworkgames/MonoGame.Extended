﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DeferredBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private readonly List<BatchDrawOperation> _drawOperations = new List<BatchDrawOperation>();
        private readonly BatchDrawOperation _emptyDrawOperation = new BatchDrawOperation((PrimitiveType)(-1), 0, 0, 0, 0, null);
        private BatchDrawOperation _currentOperation;
        private readonly TVertexType[] _vertices;
        private int _usedVertexCount;
        private readonly short[] _indices;
        private int _usedIndexCount;

        public DeferredBatchQueuer(BatchDrawer<TVertexType> batchDrawer)
            : base(batchDrawer)
        {
            _vertices = new TVertexType[batchDrawer.MaximumBatchSize];
            _indices = new short[batchDrawer.MaximumBatchSize];
            _currentOperation = _emptyDrawOperation;
        }

        private void CreateNewOperationIfNecessary(PrimitiveType primitiveType, int vertexCount, int indexCount, IDrawContext drawContext)
        {
            // line strip and triangle strip always need a new operation
            if (indexCount > 0 == _currentOperation.IndexCount > 0 && drawContext.Equals(_currentOperation.DrawContext) && (primitiveType == _currentOperation.PrimitiveType) && (_currentOperation.PrimitiveType != PrimitiveType.LineStrip) && (_currentOperation.PrimitiveType != PrimitiveType.TriangleStrip))
            {
                _currentOperation.VertexCount += vertexCount;
                _currentOperation.IndexCount += indexCount;
                return;
            }

            _currentOperation = new BatchDrawOperation(primitiveType, _usedVertexCount, vertexCount, _usedIndexCount, indexCount, drawContext);
            _drawOperations.Add(_currentOperation);
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

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _drawOperations.Count; ++index)
            {
                var operation = _drawOperations[index];
                if (operation.IndexCount == 0)
                {
                    BatchDrawer.Draw(operation.PrimitiveType, operation.StartVertex, operation.VertexCount, operation.DrawContext);
                }
                else
                {
                    BatchDrawer.Draw(operation.PrimitiveType, operation.StartVertex, operation.VertexCount, operation.StartIndex, operation.IndexCount, operation.DrawContext);
                }
            }

            _drawOperations.Clear();
            _currentOperation = _emptyDrawOperation;
            _usedVertexCount = 0;
            _usedIndexCount = 0;
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            var remainingSpace = BatchDrawer.MaximumBatchSize - Math.Max(_usedVertexCount, _usedIndexCount);

            if (vertexCount <= remainingSpace)
            {
                QueueVerticesNoOverflow(primitiveType, vertices, startVertex, vertexCount, drawContext);
            }
            else
            {
                QueueVerticesBufferSplit(primitiveType, vertices, startVertex, vertexCount, drawContext, remainingSpace);
            }
        }

        private void QueueVerticesNoOverflow(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            CreateNewOperationIfNecessary(primitiveType, vertexCount, 0, drawContext);
            _usedVertexCount += vertexCount;
        }

        private void QueueVerticesBufferSplit(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext, int spaceLeft)
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
                CreateNewOperationIfNecessary(primitiveType, spaceLeft, 0, drawContext);
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
                CreateNewOperationIfNecessary(primitiveType, verticesToProcess, 0, drawContext);
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

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext)
        {
            var remainingVertices = BatchDrawer.MaximumBatchSize - _usedVertexCount;
            var remainingIndices = BatchDrawer.MaximumBatchSize - _usedIndexCount;

            var exceedsBatchSpace = (vertexCount > remainingVertices) || (indexCount > remainingIndices);

            if (!exceedsBatchSpace)
            {
                QueueIndexedVerticesNoOverflow(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, drawContext);
            }
            else
            {
                var remainingSpace = Math.Min(remainingVertices, remainingIndices);
                QueueIndexedVerticesBufferSplit(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, drawContext, remainingSpace);
            }
        }

        private void QueueIndexedVerticesNoOverflow(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext)
        {
            Array.Copy(vertices, startVertex, _vertices, _usedVertexCount, vertexCount);
            var indexOffset = _currentOperation.VertexCount;
            CreateNewOperationIfNecessary(primitiveType, vertexCount, indexCount, drawContext);
            _usedVertexCount += vertexCount;

            // we can't use Array.Copy to copy the indices because we need to add the offset
            var maxIndexCount = startIndex + indexCount;
            for (var index = startIndex; index < maxIndexCount; ++index)
            {
                _indices[_usedIndexCount++] = (short)(indices[index] + indexOffset);
            }
        }

        private void QueueIndexedVerticesBufferSplit(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext context, int spaceLeft)
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
                CreateNewOperationIfNecessary(type, spaceLeft, spaceLeft, context);
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

                CreateNewOperationIfNecessary(type, verticesToProcess, indicesToProcess, context);

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