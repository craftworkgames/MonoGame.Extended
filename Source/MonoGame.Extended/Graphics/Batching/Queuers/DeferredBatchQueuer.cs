using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class DeferredBatchQueuer<TVertexType, TBatchItemData> : BatchQueuer<TVertexType, TBatchItemData>
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IDrawContext<TBatchItemData>
    {
        internal static readonly DrawContext<TBatchItemData> EmptyDrawContext = new DrawContext<TBatchItemData>((PrimitiveType)(-1), 0, 0, default(TBatchItemData));

        private const int InitialOperationsCapacity = 25;

        // the draw operations buffer
        private DrawContext<TBatchItemData>[] _contexts;
        // the sort keys buffer for the draw operations
        // the keys are seperated from the draw operations to have the smallest memory footprint possible for each draw item
        private uint[] _itemSortKeys;
        // the number of operations in the buffers
        private int _itemsCount;
        // the current draw item
        private DrawContext<TBatchItemData> _currentContext;
        // the sort key for the current draw item
        private uint _currentSortKey;

        private readonly GeometryBuffer<TVertexType> _geometryBuffer;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType, TBatchItemData> batchDrawer)
            : base(batchDrawer)
        {
            _currentContext = EmptyDrawContext;
            _itemSortKeys = new uint[InitialOperationsCapacity];
            _contexts = new DrawContext<TBatchItemData>[InitialOperationsCapacity];
            _geometryBuffer = batchDrawer.GeometryBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyCurrentItem()
        {
            _contexts[_itemsCount - 1] = _currentContext;
        }

        internal override void Begin(Effect effect)
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
            if (_geometryBuffer.VertexCount == 0 || _geometryBuffer.IndexCount == 0)
            {
                return;
            }

            BatchDrawer.Select(_geometryBuffer.VertexCount, _geometryBuffer.IndexCount);

            ApplyCurrentItem();

            // sort only the items which are used
            PrimitiveBatchHelper.SortAction?.Invoke(_itemSortKeys, _contexts, 0, _itemsCount);

            for (var index = 0; index < _itemsCount; index++)
            {
                var item = _contexts[index];
                var primitiveType = (PrimitiveType)item.PrimitiveType;

                BatchDrawer.Draw(primitiveType, item.StartIndex, item.PrimitiveCount, ref item.Data);

                // clear the data for the item because it may contain references to objects like a Texture 
                // values like startVertex, vertexCount, etc will be overwritten and don't need to be cleared 
                _contexts[index].Data = default(TBatchItemData);
            }

            if (_geometryBuffer.BufferType == GeometryBufferType.Dynamic)
            {
                _geometryBuffer.Clear();
            }

            // don't need to clear the array because we keep track of how many operations we have
            // i.e. by changing itemsCount to zero, we will overwrite items which have already been processed
#if DEBUG
            Array.Clear(_contexts, 0, _itemsCount);
#endif

            _itemsCount = 0;
            _currentContext = EmptyDrawContext;
            _currentSortKey = 0;
        }

        internal override void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0)
        {
            // "merge" multiple draw calls into one draw item if possible
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each line strip or triangle strip

            if (_currentSortKey == sortKey && _currentContext.CanMergeIntoItem(ref data, (byte)primitiveType))
            {
                _currentContext.PrimitiveCount += primitiveType.GetPrimitiveCount(indexCount);
                return;
            }

            if (_itemsCount > 0)
            {
                ApplyCurrentItem();
            }

            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            _currentContext = new DrawContext<TBatchItemData>(primitiveType, startIndex, primitiveCount, data);

            if (_itemsCount >= _contexts.Length)
            {
                // increase draw item buffers by the golden ratio
                var newCapacity = (int)(_contexts.Length * 1.61803398875f);
                Array.Resize(ref _itemSortKeys, newCapacity);
                Array.Resize(ref _contexts, newCapacity);
            }

            _itemSortKeys[_itemsCount] = sortKey;
            _contexts[_itemsCount++] = _currentContext;
            _currentSortKey = sortKey;
        }
    }
}
