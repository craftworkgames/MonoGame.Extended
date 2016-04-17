using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal readonly int MaximumBatchSize;

        protected BatchDrawer(GraphicsDevice graphicsDevice, int maximumBatchSize)
        {
            GraphicsDevice = graphicsDevice;
            MaximumBatchSize = maximumBatchSize;
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
        }

        internal abstract void Begin(Effect effect, TVertexType[] vertices);
        internal abstract void Begin(Effect effect, TVertexType[] vertices, short[] indices);
        internal abstract void End();
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount);
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount);
    }
}
