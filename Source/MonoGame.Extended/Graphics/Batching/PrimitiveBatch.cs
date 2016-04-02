using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class PrimitiveBatch<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        private IBatcher<TVertexType> _batcher; 

        public PrimitiveBatch(IBatcher<TVertexType> batcher)
        {
            _batcher = batcher;
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

            _batcher.Dispose();
            _batcher = null;
        }

        public void Begin()
        {
        }

        public void End()
        {  
        }

        public void Draw(PrimitiveType primitiveType, TVertexType vertices, IDrawContext context)
        { 
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext context)
        {
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, short[] indices, IDrawContext context)
        {
        }

        public void Draw(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext context)
        {
        }
    }
}
