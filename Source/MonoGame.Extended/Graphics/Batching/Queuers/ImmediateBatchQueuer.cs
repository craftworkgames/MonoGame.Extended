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

        internal override void EnqueueDraw(int startIndex, int indexCount, ref TDrawContext context, uint sortKey = 0)
        {
            BatchDrawer.SelectBuffers();
            var primitiveCount = BatchDrawer.PrimitiveType.GetPrimitiveCount(indexCount);
            BatchDrawer.Draw(startIndex, primitiveCount, ref context);
            BatchDrawer.GeometryBuffer.Clear();
        }
    }
}
