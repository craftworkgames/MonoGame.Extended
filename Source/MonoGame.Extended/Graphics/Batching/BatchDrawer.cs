using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal IDrawContext DefaultDrawContext;
        internal readonly int MaxiumumBatchSize;

        protected BatchDrawer(GraphicsDevice graphicsDevice, IDrawContext defaultDrawContext, int maxiumumBatchSize)
        {
            GraphicsDevice = graphicsDevice;
            DefaultDrawContext = defaultDrawContext;
            MaxiumumBatchSize = maxiumumBatchSize;
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

            GraphicsDevice = null;
            DefaultDrawContext = null;
        }

        internal abstract void Select(TVertexType[] vertices);
        internal abstract void Select(TVertexType[] vertices, short[] indices);
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext);
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext);
    }
}
