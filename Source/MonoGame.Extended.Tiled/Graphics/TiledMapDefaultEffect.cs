using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Graphics
{
    public class TiledMapDefaultEffect : BasicEffect, ITiledMapEffect
    {
        public TiledMapDefaultEffect(GraphicsDevice device) 
            : base(device)
        {
            TextureEnabled = true;
        }

        public TiledMapDefaultEffect(BasicEffect cloneSource) 
            : base(cloneSource)
        {
            TextureEnabled = true;
        }
    }
}
