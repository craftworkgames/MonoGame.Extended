using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchQueuer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal BatchDrawer<TVertexType> BatchDrawer;
        internal PrimitiveType PrimitiveType;

        internal abstract void Begin();
        internal abstract void End();
        internal abstract void EnqueueDraw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortKey = 0);
        internal abstract void EnqueueDraw(IDrawContext drawContext, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey = 0);

        protected BatchQueuer(BatchDrawer<TVertexType> batchDrawer)
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
