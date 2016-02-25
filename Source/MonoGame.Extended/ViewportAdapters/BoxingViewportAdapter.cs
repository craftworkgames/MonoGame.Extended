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

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxingViewportAdapter"/>. 
        /// Note: If you're using DirectX please use the other constructor due to a bug in MonoGame.
        /// https://github.com/mono/MonoGame/issues/4018
        /// </summary>
        public BoxingViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
            : base(graphicsDevice, virtualWidth, virtualHeight)
        {
            _window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxingViewportAdapter"/>. 
        /// Use this constructor only if you're using DirectX due to a bug in MonoGame.
        /// https://github.com/mono/MonoGame/issues/4018
        /// This constructor will be made obsolete and eventually removed once the bug has been fixed.
        /// </summary>
        public BoxingViewportAdapter(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, int virtualWidth, int virtualHeight)
            : this(window, graphicsDeviceManager.GraphicsDevice, virtualWidth, virtualHeight)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
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

            // Needed for a DirectX bug in MonoGame 3.4. Hopefully it will be fixed in future versions
            // see http://gamedev.stackexchange.com/questions/68914/issue-with-monogame-resizing
            if (_graphicsDeviceManager != null &&
                    (_graphicsDeviceManager.PreferredBackBufferWidth != _window.ClientBounds.Width ||
                    _graphicsDeviceManager.PreferredBackBufferHeight != _window.ClientBounds.Height))
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = _window.ClientBounds.Width;
                _graphicsDeviceManager.PreferredBackBufferHeight = _window.ClientBounds.Height;
                _graphicsDeviceManager.ApplyChanges();
            }
        }

        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public override Point PointToScreen(int x, int y)
        {
            var viewport = GraphicsDevice.Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }
    }
}