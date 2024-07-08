using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Tiled.Renderers
{
    public interface ITiledMapEffect : IEffectMatrices, ITextureEffect
    {
        float Alpha { get; set; }
    }

    public class TiledMapEffect : DefaultEffect, ITiledMapEffect
    {
        public TiledMapEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Initialize();
        }

        public TiledMapEffect(GraphicsDevice graphicsDevice, byte[] byteCode)
            : base(graphicsDevice, byteCode)
        {
            Initialize();
        }

        public TiledMapEffect(Effect cloneSource)
            : base(cloneSource)
        {
            Initialize();
        }

        private void Initialize()
        {
            VertexColorEnabled = false;
            TextureEnabled = true;
        }
    }
}
