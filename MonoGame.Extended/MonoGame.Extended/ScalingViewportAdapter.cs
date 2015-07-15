using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        public ScalingViewportAdapter(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight) 
            : base(graphicsDevice)
        {
            _virtualWidth = virtualWidth;
            _virtualHeight = virtualHeight;
        }

        private readonly int _virtualWidth;
        public override int VirtualWidth
        {
            get { return _virtualWidth; }
        }

        private readonly int _virtualHeight;
        public override int VirtualHeight
        {
            get { return _virtualHeight; }
        }

        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ActualWidth / VirtualWidth;
            var scaleY = (float)ActualHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}