using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchQueuer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal BatchDrawer<TVertexType> BatchDrawer;  

        internal abstract void Begin(IDrawContext drawContext);
        internal abstract void End();
        internal abstract void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortkey);
        internal abstract void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey);

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
