using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchDrawer<TVertexType, TDrawContext> : IDisposable
        where TVertexType : struct, IVertexType where TDrawContext : struct, IDrawContext
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

        internal void Draw(int startIndex, int primitiveCount, ref TDrawContext data)
        {
            var graphicsDevice = GraphicsDevice;
            var effect = Effect;
            data.Apply(effect);
            var passes = effect.CurrentTechnique.Passes;
            for (var passIndex = 0; passIndex < passes.Count; passIndex++)
            {
                var pass = passes[passIndex];
                pass.Apply();
                data.AfterApplyPass(passIndex, effect);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType, 0, startIndex, primitiveCount);
            }
        }
    }
}
