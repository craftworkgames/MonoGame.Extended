using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching.Drawers;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal abstract class BatchCommandQueuer<TVertexType, TCommandContext> : IDisposable
        where TVertexType : struct, IVertexType where TCommandContext : struct, IBatchCommandContext
    {
        internal BatchDrawer<TVertexType, TCommandContext> BatchDrawer;
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

        internal abstract void EnqueueDraw(ushort startIndex, ushort indexCount, ref TCommandContext context, uint sortKey = 0);

        protected BatchCommandQueuer(BatchDrawer<TVertexType, TCommandContext> batchDrawer)
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
