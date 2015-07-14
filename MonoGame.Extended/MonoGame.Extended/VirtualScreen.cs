using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class VirtualScreen
    {
        private readonly GraphicsDevice _graphicsDevice;

        public VirtualScreen(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
        {
            _graphicsDevice = graphicsDevice;

            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public int VirtualWidth { get; private set; }
        public int VirtualHeight { get; private set; }

        public int ActualWidth
        {
            get { return _graphicsDevice.Viewport.Width; }
        }

        public int ActualHeight
        {
            get { return _graphicsDevice.Viewport.Height; }
        }

        public Matrix GetScaleMatrix()
        {
            var scaleX = (float) ActualWidth / VirtualWidth;
            var scaleY = (float) ActualHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }

        public void OnClientSizeChanged()
        {
            var aspectRatio = (float) VirtualWidth / VirtualHeight;
            var newWidth = _graphicsDevice.Viewport.Width;
            var newHeight = _graphicsDevice.Viewport.Height;
            _graphicsDevice.Viewport = new Viewport(100, 100, 300, 300);
        }
    }
}