using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class UserPrimitivesBatchDrawer<TVertexType> : BatchDrawer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private TVertexType[] _vertices;
        private short[] _indices;

        internal UserPrimitivesBatchDrawer(GraphicsDevice graphicsDevice, IDrawContext defaultDrawContext, int maximumBatchSize = PrimitiveBatch<TVertexType>.DefaultMaximumBatchSize)
            : base(graphicsDevice, defaultDrawContext, maximumBatchSize)
        {
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            _vertices = null;
            _indices = null;
        }

        internal override void Select(TVertexType[] vertices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            _vertices = vertices;
        }

        internal override void Select(TVertexType[] vertices, short[] indices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (indices == null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            _vertices = vertices;
            _indices = indices;
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            if (drawContext == null)
            {
                drawContext = DefaultDrawContext;
            }

            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            var passesCount = drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                drawContext.ApplyPass(passIndex);
                GraphicsDevice.DrawUserPrimitives(primitiveType, _vertices, startVertex, primitiveCount);
            }
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            if (drawContext == null)
            {
                drawContext = DefaultDrawContext;
            }

            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            var passesCount = drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                drawContext.ApplyPass(passIndex);

                GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, primitiveCount);
            }
        }
    }
}
