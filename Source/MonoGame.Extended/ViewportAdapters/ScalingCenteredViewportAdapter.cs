using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

// ReSharper disable once CheckNamespace

namespace MonoGame.Extended.ViewportAdapters
{
    public class ScalingCenteredViewportAdapter : ScalingViewportAdapter
    {
        protected readonly GameWindow Window;
        protected Matrix _transform;

        public ScalingCenteredViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
            : base(graphicsDevice, virtualWidth, virtualHeight)
        {
            Window = window;
            window.ClientSizeChanged += OnClientSizeChanged;

            ComputeMatrix();
        }

        public override int ViewportWidth => Window.ClientBounds.Width;
        public override int ViewportHeight => Window.ClientBounds.Height;
        public float MarginWidth { get; protected set; }
        public float MarginHeight { get; protected set; }

        public override Matrix GetScaleMatrix()
        {
            return _transform;
        }

        public override Point PointToScreen(int x, int y)
        {
            var viewport = GraphicsDevice.Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var x = Window.ClientBounds.Width;
            var y = Window.ClientBounds.Height;

            GraphicsDevice.Viewport = new Viewport(0, 0, x, y);

            ComputeMatrix();
        }

        protected void ComputeMatrix()
        {
            var viewport = GraphicsDevice.Viewport;

            var actualScale = (float)viewport.Width / viewport.Height;
            var wantedScale = (float)VirtualWidth / VirtualHeight;

            Vector2 scale = Vector2.Zero;

            if (actualScale == wantedScale)
            {
                scale = new Vector2((float)viewport.Width / VirtualWidth, (float)viewport.Height / VirtualHeight);
            }
            else if (actualScale > wantedScale)
            {
                float wantedWidth = ((float)viewport.Height / VirtualHeight) * VirtualWidth;
                float totalMargin = viewport.Width - wantedWidth;

                scale = new Vector2((float)wantedWidth / VirtualWidth, (float)viewport.Height / VirtualHeight);

                MarginWidth = totalMargin / 2;
                MarginHeight = 0;
            }
            else if (actualScale < wantedScale)
            {
                float wantedHeight = ((float)viewport.Width / VirtualWidth) * VirtualHeight;
                float totalMargin = viewport.Height - wantedHeight;

                scale = new Vector2((float)viewport.Width / VirtualWidth, (float)wantedHeight / VirtualHeight);

                MarginWidth = 0;
                MarginHeight = totalMargin / 2;
            }

            _transform = Matrix.CreateScale(scale.X, scale.Y, 1.0f)
                * Matrix.CreateTranslation(MarginWidth, MarginHeight, 0);
        }
    }
}