using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DynamicVertexBufferBatchDrawer<TVertexType> : BatchDrawer<TVertexType> where TVertexType : struct,IVertexType
    {
        internal DynamicVertexBufferBatchDrawer(GraphicsDevice graphicsDevice, IDrawContext defaultDrawContext, int maximumBatchSize)
            : base(graphicsDevice, defaultDrawContext, maximumBatchSize)
        {
        }

        internal override void Select(TVertexType[] vertices)
        {
            throw new NotImplementedException();
        }

        internal override void Select(TVertexType[] vertices, short[] indices)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            throw new NotImplementedException();
        }
    }
}
