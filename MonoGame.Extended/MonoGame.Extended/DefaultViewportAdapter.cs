using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
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
            get { return _graphicsDevice.Viewport.Width; }
        }

        public override int ActualWidth
        {
            get { return _graphicsDevice.Viewport.Width; }
        }

        public override int ActualHeight
        {
            get { return _graphicsDevice.Viewport.Width; }
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