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
        }

        internal override void End()
        {
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, IDrawContext drawContext = null)
        {
            BatchDrawer.Select(vertices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount);
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, IDrawContext drawContext = null)
        {
            BatchDrawer.Select(vertices, indices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount, startIndex, indexCount);
        }
    }
}
