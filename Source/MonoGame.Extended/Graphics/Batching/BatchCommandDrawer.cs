using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchCommandDrawer<TVertexType, TIndexType, TCommandData> : IDisposable
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
        where TIndexType : struct
    {
        internal readonly GeometryBuffer<TVertexType, TIndexType> GeometryBuffer;
        internal readonly GraphicsDevice GraphicsDevice;
        internal Effect Effect;
        internal PrimitiveType PrimitiveType;

        internal BatchCommandDrawer(GraphicsDevice graphicsDevice,
            GeometryBuffer<TVertexType, TIndexType> geometryBuffer)
        {
            GraphicsDevice = graphicsDevice;
            GeometryBuffer = geometryBuffer;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            GeometryBuffer.Dispose();
        }

        internal void SelectBuffers()
        {
            GeometryBuffer.Flush();
            GraphicsDevice.SetVertexBuffer(GeometryBuffer.VertexBuffer);
            GraphicsDevice.Indices = GeometryBuffer.IndexBuffer;
        }

        internal void Draw(ref BatchDrawCommand<TCommandData> command)
        {
            command.Data.ApplyTo(Effect);
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType, 0, command.StartIndex,
                    command.PrimitivesCount);
            }
            command.Data.SetReferencesToNull();
        }
    }
}