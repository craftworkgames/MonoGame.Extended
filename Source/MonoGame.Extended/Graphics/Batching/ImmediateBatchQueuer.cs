using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class ImmediateBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private IDrawContext _drawContext;

        public ImmediateBatchQueuer(BatchDrawer<TVertexType> batchDrawer)
            : base(batchDrawer)
        {
        }

        internal override void Begin(IDrawContext drawContext)
        {
            Debug.Assert(drawContext != null);
            _drawContext = drawContext;
        }

        internal override void End()
        {
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortkey)
        {
            BatchDrawer.Begin(_drawContext, vertices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount);
            BatchDrawer.End();
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey)
        {
            BatchDrawer.Begin(_drawContext, vertices, indices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount, startIndex, indexCount);
            BatchDrawer.End();
        }
    }
}
