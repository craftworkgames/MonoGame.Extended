using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal readonly ushort MaximumVerticesCount;
        internal readonly ushort MaximumIndicesCount;
        internal List<Action> CommandDelegates;

        protected BatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = PrimitiveBatch<TVertexType>.DefaultMaximumVerticesCount, ushort maximumIndicesCount = PrimitiveBatch<TVertexType>.DefaultMaximumIndicesCount)
        {
            GraphicsDevice = graphicsDevice;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            CommandDelegates = new List<Action>();
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
        internal abstract void Draw(Effect effect, PrimitiveType primitiveType, int startVertex, int vertexCount);
        internal abstract void Draw(Effect effect, PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount);
    }
}
