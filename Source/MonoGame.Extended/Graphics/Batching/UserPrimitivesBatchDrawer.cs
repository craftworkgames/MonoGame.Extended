using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class UserPrimitivesBatchDrawer<TVertexType> : BatchDrawer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private TVertexType[] _vertices;
        private short[] _indices;

        internal UserPrimitivesBatchDrawer(GraphicsDevice graphicsDevice, int maximumBatchVerticesSizeKiloBytes = PrimitiveBatch<TVertexType>.DefaultMaximumBatchVerticesSizeKiloBytes, int maximumBatchIndicesSizeKiloBytes = PrimitiveBatch<TVertexType>.DefaultMaximumBatchVerticesSizeKiloBytes)
            : base(graphicsDevice, maximumBatchVerticesSizeKiloBytes, maximumBatchIndicesSizeKiloBytes)
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
            Debug.Assert(vertices != null);
            _vertices = vertices;
        }

        internal override void Select(TVertexType[] vertices, short[] indices)
        {
            Debug.Assert(vertices != null);
            Debug.Assert(indices != null);

            _vertices = vertices;
            _indices = indices;
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            foreach (var pass in drawContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(primitiveType, _vertices, startVertex, primitiveCount);
            }
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            foreach (var pass in drawContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, primitiveCount);
            }
        }
    }
}
