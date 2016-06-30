using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal class ImmediateBatchQueuer<TVertexType, TBatchItemData> : BatchQueuer<TVertexType, TBatchItemData>
        where TVertexType : struct, IVertexType
        where TBatchItemData : struct, IBatchItemData<TBatchItemData>
    {
        public ImmediateBatchQueuer(BatchDrawer<TVertexType, TBatchItemData> batchDrawer)
            : base(batchDrawer)
        {
        }

        internal override void Begin(Effect effect)
        {
            BatchDrawer.Effect = effect;
        }

        internal override void End()
        {
            BatchDrawer.Effect = null;
        }

        internal override void EnqueueDraw(ref TBatchItemData data, PrimitiveType primitiveType, TVertexType[] vertices, int startVertex, int vertexCount, int[] indices, int startIndex, int indexCount, uint sortKey = 0)
        {
            BatchDrawer.Select(vertices, startVertex, vertexCount, indices, startIndex, indexCount);
            BatchDrawer.Draw(ref data, primitiveType, startVertex, startVertex + vertexCount, startIndex, indexCount);
        }
    }
}
