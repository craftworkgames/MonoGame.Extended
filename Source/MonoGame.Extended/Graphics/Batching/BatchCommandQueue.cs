using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchCommandQueue<TVertexType, TCommandData> : IDisposable
        where TVertexType : struct, IVertexType where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal BatchDrawer<TVertexType, TCommandData> BatchDrawer;
        internal PrimitiveType PrimitiveType;

        internal virtual void Begin(Effect effect, PrimitiveType primitiveType)
        {
            BatchDrawer.Effect = effect;
            BatchDrawer.PrimitiveType = primitiveType;
            PrimitiveType = primitiveType;
        }

        protected internal abstract void Flush();

        internal void End()
        {
            Flush();
            BatchDrawer.Effect = null;
        }

        internal abstract void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, uint sortKey, ref TCommandData data);

        protected BatchCommandQueue(BatchDrawer<TVertexType, TCommandData> batchDrawer)
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
