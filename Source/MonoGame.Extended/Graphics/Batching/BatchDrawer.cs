using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal GraphicsDevice GraphicsDevice;
        internal RenderGeometryBuffer<TVertexType> GeometryBuffer;
        internal DynamicVertexBuffer VertexBuffer;
        internal DynamicIndexBuffer IndexBuffer;
        internal Effect Effect;

        internal BatchDrawer(GraphicsDevice graphicsDevice, RenderGeometryBuffer<TVertexType> geometryBuffer)
        {
            GraphicsDevice = graphicsDevice;
            GeometryBuffer = geometryBuffer;
            VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(TVertexType), geometryBuffer.Vertices.Length, BufferUsage.WriteOnly);
            IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(int), geometryBuffer.Indices.Length, BufferUsage.WriteOnly);
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

        internal void Select(int vertexCount, int indexCount)
        {
            VertexBuffer.SetData(GeometryBuffer.Vertices, 0, vertexCount);
            IndexBuffer.SetData(GeometryBuffer.Indices, 0, indexCount);
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;
        }

        internal void Draw(PrimitiveType primitiveType, int startIndex, int primitiveCount, ref TBatchItemData batchItemData)
        {
            batchItemData.ApplyTo(Effect);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(primitiveType, 0, startIndex, primitiveCount);
            }
        }
    }
}
