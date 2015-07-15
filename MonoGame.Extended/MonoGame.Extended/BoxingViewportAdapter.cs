using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public enum BoxingMode
    {
        Letterbox, Pillarbox
    }

    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        public BoxingViewportAdapter(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
            : base(graphicsDevice, virtualWidth, virtualHeight)
        {
        }

        public BoxingMode BoxingMode { get; private set; }
        
        public override void OnClientSizeChanged()
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
        }
    }
}