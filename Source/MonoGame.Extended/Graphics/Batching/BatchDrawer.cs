using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType, TCommandData> : IDisposable
        where TVertexType : struct, IVertexType where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        internal readonly GraphicsDevice GraphicsDevice;
        internal readonly GeometryBuffer<TVertexType> GeometryBuffer;
        internal Effect Effect;
        internal PrimitiveType PrimitiveType;

        internal BatchDrawer(GraphicsDevice graphicsDevice, GeometryBuffer<TVertexType> geometryBuffer)
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
            {
                return;
            }

            GeometryBuffer.Dispose();
        }

        internal void SelectBuffers()
        {
            GeometryBuffer.Flush();
            GraphicsDevice.SetVertexBuffer(GeometryBuffer.VertexBuffer);
            GraphicsDevice.Indices = GeometryBuffer.IndexBuffer;
        }

        internal void Draw(ref BatchDrawCommand<TCommandData> drawCommand)
        {
            var graphicsDevice = GraphicsDevice;
            drawCommand.Data.ApplyTo(Effect);
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType, 0, drawCommand.StartIndex, drawCommand.PrimitiveCount);
            }
        }
    }
}
