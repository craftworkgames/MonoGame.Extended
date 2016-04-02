using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        private IBatchDrawer<TVertexType> _batchDrawer;
        private IBatchQueuer<TVertexType> _batchQueuer; 

        public PrimitiveBatch(IBatchDrawer<TVertexType> batchDrawer, IBatchQueuer<TVertexType> batchQueuer)
        {
            _batchDrawer = batchDrawer;
            _batchQueuer = batchQueuer;
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

            _batchDrawer.Dispose();
            _batchDrawer = null;
            _batchQueuer.Dispose();
            _batchQueuer = null;
        }

        public void Begin()
        {
            _batchQueuer.Begin();
        }

        public void End()
        {  
            _batchQueuer.End();
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, IDrawContext drawContext)
        { 
            _batchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            _batchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, IDrawContext drawContext)
        {
            _batchQueuer.Queue(primitiveType, vertices, 0, vertices.Length, indices, 0, indices.Length, drawContext);
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext)
        {
            _batchQueuer.Queue(primitiveType, vertices, startVertex, vertexCount, indices, startIndex, indexCount, drawContext);
        }
    }
}
