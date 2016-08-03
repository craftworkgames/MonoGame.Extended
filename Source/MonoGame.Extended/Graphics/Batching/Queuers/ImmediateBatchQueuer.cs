using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class ImmediateBatchQueuer<TVertexType, TBatchItemData> : BatchQueuer<TVertexType, TBatchItemData>
        where TVertexType : struct, IVertexType
        where TBatchItemData : struct, IDrawContext<TBatchItemData>
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

        internal override void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TBatchItemData data, uint sortKey = 0)
        {
            BatchDrawer.Select(vertexCount, indexCount);
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);
            BatchDrawer.Draw(primitiveType, startIndex, primitiveCount, ref data);
            BatchDrawer.GeometryBuffer.Clear();
        }
    }
}
