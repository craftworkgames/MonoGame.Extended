using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public static class RenderTarget2DExtensions
    {
        public static IDisposable BeginDraw(this RenderTarget2D renderTarget, GraphicsDevice graphicsDevice,
            Color backgroundColor)
        {
            return new RenderTargetOperation(renderTarget, graphicsDevice, backgroundColor);
        }

        private class RenderTargetOperation : IDisposable
        {
            private readonly GraphicsDevice _graphicsDevice;
            private readonly RenderTargetUsage _previousRenderTargetUsage;
            private readonly Viewport _viewport;

            public RenderTargetOperation(RenderTarget2D renderTarget, GraphicsDevice graphicsDevice,
                Color backgroundColor)
            {
                _graphicsDevice = graphicsDevice;
                _viewport = _graphicsDevice.Viewport;
                _previousRenderTargetUsage = _graphicsDevice.PresentationParameters.RenderTargetUsage;

                _graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                _graphicsDevice.SetRenderTarget(renderTarget);
                _graphicsDevice.Clear(backgroundColor);
            }

            public void Dispose()
            {
                _graphicsDevice.SetRenderTarget(null);
                _graphicsDevice.PresentationParameters.RenderTargetUsage = _previousRenderTargetUsage;
                _graphicsDevice.Viewport = _viewport;
            }
        }
    }
}