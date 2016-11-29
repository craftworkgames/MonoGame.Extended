using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchCommandQueue<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct, IVertexType where TIndexType : struct where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal BatchCommandDrawer<TVertexType, TIndexType, TCommandData> CommandDrawer;
        internal GraphicsDevice GraphicsDevice;
        internal GraphicsGeometryData<TVertexType, TIndexType> GeometryData;

        protected BatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> commandDrawer, GraphicsGeometryData<TVertexType, TIndexType> geometryData)
        {
            GraphicsDevice = graphicsDevice;
            CommandDrawer = commandDrawer;
            GeometryData = geometryData;
        }

        internal virtual void Begin(Effect effect)
        {
            CommandDrawer.Effect = effect;
        }

        protected internal abstract void Flush();

        internal void End()
        {
            Flush();
            CommandDrawer.Effect = null;
        }

        internal abstract void EnqueueDrawCommand(PrimitiveType primitiveType, int primitiveCount, int startIndex, float sortKey,
            ref TCommandData data);
    }
}