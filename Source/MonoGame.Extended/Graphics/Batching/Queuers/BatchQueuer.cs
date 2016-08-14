using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal abstract class BatchQueuer<TVertexType, TDrawContext> : IDisposable
        where TVertexType : struct, IVertexType where TDrawContext : struct, IDrawContext<TDrawContext>
    {
        internal BatchDrawer<TVertexType, TDrawContext> BatchDrawer;
        internal PrimitiveType PrimitiveType;

        internal abstract void Begin(Effect effect);
        internal abstract void End();
        internal abstract void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TDrawContext context, uint sortKey = 0);

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
