using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Extended.ViewportAdapters
{
    public class WindowViewportAdapter : ViewportAdapter
    {
        public WindowViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
        }

        protected readonly GameWindow Window;
        protected GraphicsDevice GraphicsDevice;

        public override int ViewportWidth
        {
            get { return Window.ClientBounds.Width; }
        }
        public override int ViewportHeight
        {
            get { return Window.ClientBounds.Height; }
        }

        public override int VirtualWidth
        {
            get { return Window.ClientBounds.Width; }
        }

        public override int VirtualHeight
        {
            get { return Window.ClientBounds.Height; }
        }

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var x = Window.ClientBounds.Width;
            var y = Window.ClientBounds.Height;

            GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
        }

    }
}
