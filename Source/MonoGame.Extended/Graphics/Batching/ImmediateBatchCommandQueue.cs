using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class ImmediateBatchCommandQueue<TVertexType, TIndexType, TCommandData> :
            BatchCommandQueue<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
        where TIndexType : struct
    {
        public ImmediateBatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> batchCommandDrawer)
            : base(graphicsDevice, batchCommandDrawer)
        {
        }

        protected internal override void Flush()
        {
        }

        internal override void EnqueueDrawCommand(int startIndex, int primitiveCount, float sortKey,
            ref TCommandData data)
        {
            CommandDrawer.SelectBuffers();
            var command = new BatchDrawCommand<TCommandData>(startIndex, primitiveCount, sortKey, data);
            CommandDrawer.Draw(ref command);
            CommandDrawer.GeometryBuffer.Clear();
        }
    }
}