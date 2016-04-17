using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DynamicVertexBufferBatchDrawer<TVertexType> : BatchDrawer<TVertexType> where TVertexType : struct,IVertexType
    {
        internal DynamicVertexBufferBatchDrawer(GraphicsDevice graphicsDevice, int maximumBatchSize)
            : base(graphicsDevice, maximumBatchSize)
        {
        }

        internal override void Begin(IDrawContext drawContext, TVertexType[] vertices)
        {
            throw new NotImplementedException();
        }

        internal override void Begin(IDrawContext drawContext, TVertexType[] vertices, short[] indices)
        {
            throw new NotImplementedException();
        }

        internal override void End()
        {
            throw new NotImplementedException();
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            throw new NotImplementedException();
        }
    }
}
