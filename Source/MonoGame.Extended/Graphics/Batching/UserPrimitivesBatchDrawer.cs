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
        private Effect _effect;

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

        internal override void Begin(Effect effect, TVertexType[] vertices)
        {
            Debug.Assert(effect != null);
            Debug.Assert(vertices != null);

            _effect = effect;
            _vertices = vertices;
        }

        internal override void Begin(Effect effect, TVertexType[] vertices, short[] indices)
        {
            Debug.Assert(effect != null);
            Debug.Assert(vertices != null);
            Debug.Assert(indices != null);

            _effect = effect;
            _vertices = vertices;
            _indices = indices;
        }

        internal override void End()
        {
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(primitiveType, _vertices, startVertex, primitiveCount);
            }
        }

        internal override void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount)
        {
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, primitiveCount);
            }
        }
    }
}
