using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class DeferredBatchQueuer<TVertexType, TDrawContext> : BatchQueuer<TVertexType, TDrawContext>
        where TVertexType : struct, IVertexType where TDrawContext : struct, IDrawContext<TDrawContext>
    {
        internal static readonly BatchItem<TDrawContext> EmptyContext = new BatchItem<TDrawContext>(0, 0, 0, default(TDrawContext));

        private const int InitialOperationsCapacity = 25;

        private BatchItem<TDrawContext>[] _items;
        private int _itemsCount;
        private BatchItem<TDrawContext> _currentItem;
        private PrimitiveType _primitiveType;

        private readonly GeometryBuffer<TVertexType> _geometryBuffer;

        internal DeferredBatchQueuer(BatchDrawer<TVertexType, TDrawContext> batchDrawer)
            : base(batchDrawer)
        {
            _currentItem = EmptyContext;
            _items = new BatchItem<TDrawContext>[InitialOperationsCapacity];
            _geometryBuffer = batchDrawer.GeometryBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ApplyCurrentItem()
        {
            _items[_itemsCount - 1] = _currentItem;
        }

        internal override void Begin(Effect effect, PrimitiveType primitiveType)
        {
            _primitiveType = primitiveType;
            base.Begin(effect, primitiveType);
        }

        internal override void End()
        {
            Flush();
            base.End();
        }

        private void Flush()
        {
            if (_geometryBuffer.VertexCount == 0 || _geometryBuffer.IndexCount == 0)
            {
                return;
            }

            BatchDrawer.SelectBuffers();

            ApplyCurrentItem();

            Array.Sort(_items, 0, _itemsCount);

            for (var index = 0; index < _itemsCount; index++)
            {
                var item = _items[index];

                BatchDrawer.Draw(item.StartIndex, item.PrimitiveCount, ref item.Context);

                // clear the data for the item because it may contain references to objects like a Texture 
                // values like startVertex, vertexCount, etc will be overwritten and don't need to be cleared 
                _items[index].Context = default(TDrawContext);
            }

            if (_geometryBuffer.BufferType == GeometryBufferType.Dynamic)
            {
                _geometryBuffer.Clear();
            }

            // don't need to clear the array because we keep track of how many operations we have
            // i.e. by changing itemsCount to zero, we will overwrite items which have already been processed
#if DEBUG
            Array.Clear(_items, 0, _itemsCount);
#endif

            _itemsCount = 0;
            _currentItem = EmptyContext;
        }

        internal override void EnqueueDraw(int startIndex, int indexCount, ref TDrawContext context, uint sortKey = 0)
        {
            // "merge" multiple draw calls into one draw item if possible
            // we do not support merging line strip or triangle strip primitives, i.e., a new draw call is needed for each line strip or triangle strip

            if (_currentItem.CanMergeWith(sortKey, ref context))
            {
                _currentItem.PrimitiveCount += _primitiveType.GetPrimitiveCount(indexCount);
                return;
            }

            if (_itemsCount > 0)
            {
                ApplyCurrentItem();
            }

            var primitiveCount = _primitiveType.GetPrimitiveCount(indexCount);
            _currentItem = new BatchItem<TDrawContext>(sortKey, startIndex, primitiveCount, context);

            if (_itemsCount >= _items.Length)
            {
                // increase draw item buffers by the golden ratio
                var newCapacity = (int)(_items.Length * 1.61803398875f);
                Array.Resize(ref _items, newCapacity);
            }

            _items[_itemsCount++] = _currentItem;
        }
    }
}
