using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
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
