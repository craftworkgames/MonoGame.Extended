using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class ViewportAdapter
    {
        public ViewportAdapter(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        protected GraphicsDevice GraphicsDevice { get; private set; }

        public virtual int VirtualWidth
        {
            get { return ActualWidth; }
        }

        public virtual int VirtualHeight
        {
            get { return ActualHeight; }
        }

        public int ActualWidth
        {
            get { return GraphicsDevice.Viewport.Width; }
        }

        public int ActualHeight
        {
            get { return GraphicsDevice.Viewport.Height; }
        }

        public virtual void OnClientSizeChanged()
        {
        }

        public virtual Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}