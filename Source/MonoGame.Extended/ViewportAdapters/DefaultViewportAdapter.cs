using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace MonoGame.Extended.ViewportAdapters
{
    public class DefaultViewportAdapter : ViewportAdapter
    {
        private readonly GraphicsDevice _graphicsDevice;

        public DefaultViewportAdapter(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override int VirtualWidth
        {
            get { return _graphicsDevice.Viewport.Width; }
        }

        public override int VirtualHeight
        {
            get { return _graphicsDevice.Viewport.Height; }
        }

        public override int ViewportWidth
        {
            get { return _graphicsDevice.Viewport.Width; }
        }

        public override int ViewportHeight
        {
            get { return _graphicsDevice.Viewport.Height; }
        }

        public override void OnClientSizeChanged()
        {
        }

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}