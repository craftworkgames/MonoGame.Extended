using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace Demo.TiledMaps
{
    public class CustomEffect : DefaultEffect, ITiledMapEffect
    {
        public CustomEffect(GraphicsDevice graphicsDevice) 
            : base(graphicsDevice)
        {
        }

        public CustomEffect(GraphicsDevice graphicsDevice, byte[] byteCode) 
            : base(graphicsDevice, byteCode)
        {

        }

        public CustomEffect(Effect cloneSource) 
            : base(cloneSource)
        {
        }
    }
}
