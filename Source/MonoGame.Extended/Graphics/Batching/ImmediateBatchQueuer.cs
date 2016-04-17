using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class ImmediateBatchQueuer<TVertexType> : BatchQueuer<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private Effect _effect;

        public ImmediateBatchQueuer(BatchDrawer<TVertexType> batchDrawer)
            : base(batchDrawer)
        {
        }

        internal override void Begin(Effect effect)
        {
            Debug.Assert(effect != null);
            _effect = effect;
        }

        internal override void End()
        {
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, uint sortkey)
        {
            BatchDrawer.Begin(_effect, vertices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount);
            BatchDrawer.End();
        }

        internal override void Queue(PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, short[] indices, int startIndex, int indexCount, uint sortKey)
        {
            BatchDrawer.Begin(_effect, vertices, indices);
            BatchDrawer.Draw(primitiveType, startVertex, startVertex + vertexCount, startIndex, indexCount);
            BatchDrawer.End();
        }
    }
}
