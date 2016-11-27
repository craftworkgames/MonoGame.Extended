using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class ImmediateBatchCommandQueue<TVertexType, TCommandData> :
            BatchCommandQueue<TVertexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        public ImmediateBatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TCommandData> batchCommandDrawer)
            : base(graphicsDevice, batchCommandDrawer)
        {
        }

        protected internal override void Flush()
        {
        }

        internal override void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, float sortKey,
            ref TCommandData data)
        {
            CommandDrawer.SelectBuffers();
            var command = new BatchDrawCommand<TCommandData>(startIndex, primitiveCount, sortKey, data);
            CommandDrawer.Draw(ref command);
            CommandDrawer.GeometryBuffer.Clear();
        }
    }
}