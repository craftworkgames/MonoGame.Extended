using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class ImmediateBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        public ImmediateBatchQueuer(BatchDrawer<TVertexType> batchDrawer)
            : base(batchDrawer)
        {
        }

        internal override void Begin()
        {
            throw new NotImplementedException();
        }

        internal override void End()
        {
            throw new NotImplementedException();
        }

        internal override void Queue(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            throw new NotImplementedException();
        }

        internal override void Queue(PrimitiveType type, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext)
        {
            throw new NotImplementedException();
        }
    }
}
