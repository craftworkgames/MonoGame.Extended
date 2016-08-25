using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching.Drawers;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class ImmediateBatchCommandQueuer<TVertexType, TCommandContext> : BatchCommandQueuer<TVertexType, TCommandContext>
        where TVertexType : struct, IVertexType
        where TCommandContext : struct, IBatchCommandContext
    {
        public ImmediateBatchCommandQueuer(BatchDrawer<TVertexType, TCommandContext> batchDrawer)
            : base(batchDrawer)
        {
        }

        protected internal override void Flush()
        {   
        }

        internal override void EnqueueDraw(ushort startIndex, ushort indexCount, ref TCommandContext context, uint sortKey = 0)
        {
            BatchDrawer.SelectBuffers();
            var primitiveCount = (ushort)BatchDrawer.PrimitiveType.GetPrimitiveCount(indexCount);
            var command = new BatchCommand<TCommandContext>(sortKey, context, startIndex, primitiveCount);
            BatchDrawer.Draw(ref command);
            BatchDrawer.GeometryBuffer.Clear();
        }
    }
}
