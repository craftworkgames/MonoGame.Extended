using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal readonly int MaximumBatchVerticesSizeKiloBytes;
        internal readonly int MaximumBatchIndicesSizeKiloBytes;
        internal readonly int MaximumVerticesCount;
        internal readonly int MaximumIndicesCount;

        protected BatchDrawer(GraphicsDevice graphicsDevice, int maximumBatchVerticesSizeKiloBytes, int maximumBatchIndicesSizeKiloBytes)
        {
            GraphicsDevice = graphicsDevice;
            MaximumBatchVerticesSizeKiloBytes = maximumBatchVerticesSizeKiloBytes;
            MaximumBatchIndicesSizeKiloBytes = maximumBatchIndicesSizeKiloBytes;

            var vertexSizeBytes = Marshal.SizeOf(default(TVertexType));
            MaximumVerticesCount = MaximumBatchVerticesSizeKiloBytes * 1024 / vertexSizeBytes;
            var indexSizeBytes = Marshal.SizeOf(default(uint));
            MaximumIndicesCount = MaximumBatchIndicesSizeKiloBytes * 1024 / indexSizeBytes;
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

        internal abstract void Select(TVertexType[] vertices);
        internal abstract void Select(TVertexType[] vertices, short[] indices);
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext = null);
        internal abstract void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext = null);
    }
}
