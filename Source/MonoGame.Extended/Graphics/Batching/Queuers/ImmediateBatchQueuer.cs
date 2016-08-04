using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class ImmediateBatchQueuer<TVertexType, TDrawContext> : BatchQueuer<TVertexType, TDrawContext>
        where TVertexType : struct, IVertexType
        where TDrawContext : struct, IDrawContext<TDrawContext>
    {
        public ImmediateBatchQueuer(BatchDrawer<TVertexType, TDrawContext> batchDrawer)
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

        internal override void EnqueueDraw(PrimitiveType primitiveType, int vertexCount, int startIndex, int indexCount, ref TDrawContext context, uint sortKey = 0)
        {
            BatchDrawer.Select(vertexCount, indexCount);
            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);
            BatchDrawer.Draw(primitiveType, startIndex, primitiveCount, ref context);
            BatchDrawer.GeometryBuffer.Clear();
        }
    }
}
