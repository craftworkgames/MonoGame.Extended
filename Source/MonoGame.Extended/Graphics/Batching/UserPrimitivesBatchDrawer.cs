using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class UserPrimitivesBatchDrawer<TVertexType> : BatchDrawer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private TVertexType[] _vertices;
        private short[] _indices;
        private IDrawContext _drawContext;

        internal UserPrimitivesBatchDrawer(GraphicsDevice graphicsDevice, int maximumBatchSize = PrimitiveBatch<TVertexType>.DefaultMaximumBatchSize)
            : base(graphicsDevice, maximumBatchSize)
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

        internal override void Begin(IDrawContext drawContext, TVertexType[] vertices)
        {
            Debug.Assert(drawContext != null);
            Debug.Assert(vertices != null);

            _drawContext = drawContext;
            _drawContext.Begin();
            _vertices = vertices;
        }

        internal override void Begin(IDrawContext drawContext, TVertexType[] vertices, short[] indices)
        {
            Debug.Assert(drawContext != null);
            Debug.Assert(vertices != null);
            Debug.Assert(indices != null);

            _drawContext = drawContext;
            _drawContext.Begin();
            _vertices = vertices;
            _indices = indices;
        }

        internal override void End()
        {
            _drawContext.End();
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            var passesCount = _drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                _drawContext.ApplyPass(passIndex);
                GraphicsDevice.DrawUserPrimitives(primitiveType, _vertices, startVertex, primitiveCount);
            }
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            var passesCount = _drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                _drawContext.ApplyPass(passIndex);
                GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, primitiveCount);
            }
        }
    }
}
