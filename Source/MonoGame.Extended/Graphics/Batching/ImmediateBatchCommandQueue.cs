using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class ImmediateBatchCommandQueuer<TVertexType, TCommandData> : BatchCommandQueue<TVertexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        public ImmediateBatchCommandQueuer(BatchDrawer<TVertexType, TCommandData> batchDrawer)
            : base(batchDrawer)
        {
        }

        protected internal override void Flush()
        {   
        }

        internal override void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, uint sortKey, ref TCommandData data)
        {
            BatchDrawer.SelectBuffers();
            var command = new BatchDrawCommand<TCommandData>(startIndex, primitiveCount, sortKey, data);
            BatchDrawer.Draw(ref command);
            BatchDrawer.GeometryBuffer.Clear();
        }
    }
}
