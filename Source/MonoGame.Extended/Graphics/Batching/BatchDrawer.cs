using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal readonly ushort MaximumVerticesCount;
        internal readonly ushort MaximumIndicesCount;

        internal DynamicVertexBuffer VertexBuffer;
        internal DynamicIndexBuffer IndexBuffer;

        internal BatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = PrimitiveBatch<TVertexType>.DefaultMaximumVerticesCount, ushort maximumIndicesCount = PrimitiveBatch<TVertexType>.DefaultMaximumIndicesCount)
        {
            GraphicsDevice = graphicsDevice;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof (TVertexType), maximumVerticesCount, BufferUsage.WriteOnly);
            IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof (short), maximumIndicesCount, BufferUsage.WriteOnly);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            GraphicsDevice = null;

            VertexBuffer?.Dispose();
            VertexBuffer = null;

            IndexBuffer?.Dispose();
            IndexBuffer = null;
        }
    
        internal void Select(TVertexType[] vertices, int startVertex, int vertexCount)
        {
            VertexBuffer.SetData(vertices, startVertex, vertexCount);
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
        }

        internal void Select(TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount)
        {
            VertexBuffer.SetData(vertices, startVertex, vertexCount);
            IndexBuffer.SetData(indices, startIndex, indexCount);
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;
        }

        internal void Draw(Effect effect, PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(primitiveType, startVertex, primitiveCount);
            }
        }

        internal void Draw(Effect effect, PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(primitiveType, startVertex, startIndex, primitiveCount);
            }
        }
    }
}
