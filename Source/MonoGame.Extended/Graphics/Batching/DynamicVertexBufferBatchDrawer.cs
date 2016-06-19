using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DynamicVertexBufferBatchDrawer<TVertexType> : BatchDrawer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        internal DynamicVertexBuffer VertexBuffer;
        internal DynamicIndexBuffer IndexBuffer;

        internal DynamicVertexBufferBatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = PrimitiveBatch<TVertexType>.DefaultMaximumVerticesCount, ushort maximumIndicesCount = PrimitiveBatch<TVertexType>.DefaultMaximumIndicesCount)
            : base(graphicsDevice, maximumVerticesCount, maximumIndicesCount)
        {
            VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof (TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
            IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof (short), maximumIndicesCount, BufferUsage.WriteOnly);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing)
            {
                return;
            }

            VertexBuffer?.Dispose();
            VertexBuffer = null;

            IndexBuffer?.Dispose();
            IndexBuffer = null;
        }
    
        internal override void Select(TVertexType[] vertices)
        {
            VertexBuffer.SetData(vertices);
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
        }

        internal override void Select(TVertexType[] vertices, short[] indices)
        {
            VertexBuffer.SetData(vertices);
            IndexBuffer.SetData(indices);
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;
        }

        internal override void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            ChangeDrawContextIfNecessary(drawContext);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(primitiveType, startVertex, primitiveCount);
            }
        }

        internal override void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            ChangeDrawContextIfNecessary(drawContext);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(primitiveType, startVertex, startIndex, primitiveCount);
            }
        }
    }
}
