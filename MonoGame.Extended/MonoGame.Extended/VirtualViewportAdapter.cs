using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class VirtualViewportAdapter
    {
        private readonly GraphicsDevice _graphicsDevice;

        public VirtualViewportAdapter(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
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
            var viewport = _graphicsDevice.Viewport;
            var aspectRatio = (float) VirtualWidth / VirtualHeight;
            var width = viewport.Width;
            var height = (int)(width / aspectRatio + 0.5f);

            if (height > viewport.Height)
            {
                height = viewport.Height;
                width = (int)(height * aspectRatio + 0.5f);
            }

            var x = (viewport.Width / 2) - (width / 2);
            var y = (viewport.Height / 2) - (height / 2);
            _graphicsDevice.Viewport = new Viewport(x, y, width, height);
        }
    }
}