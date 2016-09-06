using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchCommandQueue<TVertexType, TIndexType, TCommandData> : IDisposable
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
        where TIndexType : struct
    {
        internal BatchCommandDrawer<TVertexType, TIndexType, TCommandData> CommandDrawer;
        internal PrimitiveType PrimitiveType;
        internal GraphicsDevice GraphicsDevice;

        protected BatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> commandDrawer)
        {
            GraphicsDevice = graphicsDevice;
            CommandDrawer = commandDrawer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal virtual void Begin(Effect effect, PrimitiveType primitiveType)
        {
            CommandDrawer.Effect = effect;
            CommandDrawer.PrimitiveType = primitiveType;
            PrimitiveType = primitiveType;
        }

        protected internal abstract void Flush();

        internal void End()
        {
            Flush();
            CommandDrawer.Effect = null;
        }

        internal abstract void EnqueueDrawCommand(int startIndex, int primitiveCount, float sortKey,
            ref TCommandData data);

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            // don't dispose the batch drawer here; it is a shared reference
            CommandDrawer = null;
        }
    }
}