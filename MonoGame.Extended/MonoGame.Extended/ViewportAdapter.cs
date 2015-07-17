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

        public virtual int ActualWidth
        {
            get { return GraphicsDevice.Viewport.Width; }
        }

        public virtual int ActualHeight
        {
            get { return GraphicsDevice.Viewport.Height; }
        }

        public virtual void OnClientSizeChanged()
        {
        }

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        public virtual Point PointToScreen(int x, int y)
        {
            var matrix = Matrix.Invert(GetScaleMatrix());
            return Vector2.Transform(new Vector2(x, y), matrix).ToPoint();
        }

        public virtual Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}