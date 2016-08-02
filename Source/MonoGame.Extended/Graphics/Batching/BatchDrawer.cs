using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal GraphicsDevice GraphicsDevice;
        internal GeometryBuffer<TVertexType> GeometryBuffer;
        internal Effect Effect;

        internal BatchDrawer(GraphicsDevice graphicsDevice, GeometryBuffer<TVertexType> geometryBuffer)
        {
            GraphicsDevice = graphicsDevice;
            GeometryBuffer = geometryBuffer;
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

            GeometryBuffer.Dispose();
        }

        internal void Select(int vertexCount, int indexCount)
        {
            GeometryBuffer.Flush();
            GraphicsDevice.SetVertexBuffer(GeometryBuffer.VertexBuffer);
            GraphicsDevice.Indices = GeometryBuffer.IndexBuffer;
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
