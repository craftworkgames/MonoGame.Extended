using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchCommandQueue<TVertexType, TCommandData> : IDisposable
        where TVertexType : struct, IVertexType where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal BatchCommandDrawer<TVertexType, TCommandData> CommandDrawer;
        internal PrimitiveType PrimitiveType;
        internal GraphicsDevice GraphicsDevice;

        protected BatchCommandQueue(GraphicsDevice graphicsDevice, BatchCommandDrawer<TVertexType, TCommandData> commandDrawer)
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

        internal abstract void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, float sortKey, ref TCommandData data);

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            // don't dispose the batch drawer here; it is a shared reference
            CommandDrawer = null;
        }
    }
}