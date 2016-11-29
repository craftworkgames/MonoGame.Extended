using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class ImmediateBatchCommandQueue<TVertexType, TIndexType, TCommandData> :
            BatchCommandQueue<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        public ImmediateBatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> batchCommandDrawer, GraphicsGeometryData<TVertexType, TIndexType> geometryData)
            : base(graphicsDevice, batchCommandDrawer, geometryData)
        {
        }

        protected internal override void Flush()
        {
        }

        internal override void EnqueueDrawCommand(PrimitiveType primitiveType, int primitiveCount, int startIndex, float sortKey,
            ref TCommandData data)
        {

        }
    }
}