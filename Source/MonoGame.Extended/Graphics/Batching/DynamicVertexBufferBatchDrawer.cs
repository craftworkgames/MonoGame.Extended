using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class DynamicVertexBufferBatchDrawer<TVertexType> : BatchDrawer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        internal const int BufferRegionsCount = 4;

        internal DynamicVertexBuffer VertexBuffer;
        internal DynamicIndexBuffer IndexBuffer;
        internal int CurrentBufferRegionIndex;
        internal int Stride;

        internal DynamicVertexBufferBatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = PrimitiveBatch<TVertexType>.DefaultMaximumVerticesCount, ushort maximumIndicesCount = PrimitiveBatch<TVertexType>.DefaultMaximumIndicesCount)
            : base(graphicsDevice, maximumVerticesCount, maximumIndicesCount)
        {
            VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof (TVertexType), maximumVerticesCount * BufferRegionsCount, BufferUsage.WriteOnly);
            Stride = VertexBuffer.VertexDeclaration.VertexStride;
            IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof (short), maximumIndicesCount * BufferRegionsCount, BufferUsage.WriteOnly);
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
            UnselectBuffers();

            CurrentBufferRegionIndex = (CurrentBufferRegionIndex + 1) % BufferRegionsCount;
            var offset = CurrentBufferRegionIndex * MaximumVerticesCount;
            VertexBuffer.SetData(offset * Stride, vertices, 0, vertices.Length, Stride, SetDataOptions.NoOverwrite);

            SelectBuffers(CurrentBufferRegionIndex);
        }

        internal override void Select(TVertexType[] vertices, short[] indices)
        {
            UnselectBuffers();

            CurrentBufferRegionIndex = (CurrentBufferRegionIndex + 1) % BufferRegionsCount;
            var offset = CurrentBufferRegionIndex * MaximumVerticesCount;
            VertexBuffer.SetData(offset * Stride, vertices, 0, vertices.Length, Stride, SetDataOptions.NoOverwrite);
            IndexBuffer.SetData(offset * 2, indices, 0, indices.Length, SetDataOptions.NoOverwrite);

            SelectBuffers(CurrentBufferRegionIndex);
        }

        private void SelectBuffers(int bufferRegionIndex)
        {
            GraphicsDevice.Indices = IndexBuffer;
            GraphicsDevice.SetVertexBuffer(VertexBuffer, bufferRegionIndex * MaximumVerticesCount);
        }

        private void UnselectBuffers()
        {
            GraphicsDevice.Indices = null;
            GraphicsDevice.SetVertexBuffer(null);
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
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            ChangeDrawContextIfNecessary(drawContext);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(primitiveType, startVertex, startIndex, primitiveCount);
            }
        }
    }
}
