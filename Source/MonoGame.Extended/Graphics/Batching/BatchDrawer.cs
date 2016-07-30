using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType, TBatchItemData> : IDisposable
        where TVertexType : struct, IVertexType where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        internal GraphicsDevice GraphicsDevice;
        internal MeshBuffer<TVertexType> MeshBuffer;
        internal Effect Effect;

        internal BatchDrawer(GraphicsDevice graphicsDevice, MeshBuffer<TVertexType> meshBuffer)
        {
            GraphicsDevice = graphicsDevice;
            MeshBuffer = meshBuffer;
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

            MeshBuffer.Dispose();
        }

        internal void Select(int vertexCount, int indexCount)
        {
            MeshBuffer.Flush();
            GraphicsDevice.SetVertexBuffer(MeshBuffer.VertexBuffer);
            GraphicsDevice.Indices = MeshBuffer.IndexBuffer;
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
