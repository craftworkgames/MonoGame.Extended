using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal abstract class BatchQueuer<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal BatchDrawer<TVertexType, TBatchItemData> BatchDrawer;
        internal PrimitiveType PrimitiveType;

        internal abstract void Begin(Effect effect);
        internal abstract void End();
        internal abstract void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0);

        protected BatchQueuer(BatchDrawer<TVertexType, TBatchItemData> batchDrawer)
        {
            BatchDrawer = batchDrawer;
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

            // don't dispose the batch drawer here; it is a shared reference
            BatchDrawer = null;
        }
    }
}
