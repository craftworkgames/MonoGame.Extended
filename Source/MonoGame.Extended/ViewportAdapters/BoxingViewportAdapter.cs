using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace MonoGame.Extended.ViewportAdapters
{
    public enum BoxingMode
    {
        Letterbox, Pillarbox
    }

    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly GameWindow _window;

        public BoxingViewportAdapter(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, int virtualWidth, int virtualHeight)
            : base(graphicsDeviceManager.GraphicsDevice, virtualWidth, virtualHeight)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _window = window;

            window.ClientSizeChanged += OnClientSizeChanged;
        }

        public BoxingMode BoxingMode { get; private set; }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var viewport = GraphicsDevice.Viewport;
            var aspectRatio = (float) VirtualWidth / VirtualHeight;
            var width = viewport.Width;
            var height = (int)(width / aspectRatio + 0.5f);

            if (height > viewport.Height)
            {
                BoxingMode = BoxingMode.Pillarbox;
                height = viewport.Height;
                width = (int) (height * aspectRatio + 0.5f);
            }
            else
            {
                BoxingMode = BoxingMode.Letterbox;
            }

            var x = (viewport.Width / 2) - (width / 2);
            var y = (viewport.Height / 2) - (height / 2);
            GraphicsDevice.Viewport = new Viewport(x, y, width, height);

			// Needed for DirectX rendering
			// see http://gamedev.stackexchange.com/questions/68914/issue-with-monogame-resizing
			if (_graphicsDeviceManager.PreferredBackBufferWidth != _window.ClientBounds.Width || _graphicsDeviceManager.PreferredBackBufferHeight != _window.ClientBounds.Height)
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = _window.ClientBounds.Width;
				_graphicsDeviceManager.PreferredBackBufferHeight = _window.ClientBounds.Height;
				_graphicsDeviceManager.ApplyChanges();
            }
        }

        public override Point PointToScreen(int x, int y)
        {
            var viewport = GraphicsDevice.Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }
    }
}