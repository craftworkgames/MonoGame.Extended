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

        public virtual Point PointToScreen(Point point)
        {
            var matrix = Matrix.Invert(GetScaleMatrix());
            return Vector2.Transform(point.ToVector2(), matrix).ToPoint();
        }

        public Point PointToScreen(int x, int y)
        {
            return PointToScreen(new Point(x, y));
        }

        public virtual Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}