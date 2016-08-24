using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal abstract class BatchQueuer<TVertexType, TDrawContext> : IDisposable
        where TVertexType : struct, IVertexType where TDrawContext : struct, IDrawContext
    {
        internal BatchDrawer<TVertexType, TDrawContext> BatchDrawer;
        internal PrimitiveType PrimitiveType;

        internal virtual void Begin(Effect effect, PrimitiveType primitiveType)
        {
            BatchDrawer.Effect = effect;
            BatchDrawer.PrimitiveType = primitiveType;
            PrimitiveType = primitiveType;
        }

        internal virtual void End()
        {
            BatchDrawer.Effect = null;
        }

        internal abstract void EnqueueDraw(int startIndex, int indexCount, ref TDrawContext context, uint sortKey = 0);

        protected BatchQueuer(BatchDrawer<TVertexType, TDrawContext> batchDrawer)
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
