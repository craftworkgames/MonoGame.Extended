using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DynamicVertexBufferBatchDrawer<TVertexType> : BatchDrawer<TVertexType> where TVertexType : struct,IVertexType
    {
        internal DynamicVertexBufferBatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount, ushort maximumIndicesCount)
            : base(graphicsDevice, maximumVerticesCount, maximumIndicesCount)
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

        internal override void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            throw new NotImplementedException();
        }
    }
}
