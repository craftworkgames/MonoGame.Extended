using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public struct TextureRegion<TTexture>
        where TTexture : Texture
    {
        public TTexture Texture;
        public Rectangle? SourceRectangle;

        public TextureRegion(TTexture texture, Rectangle? sourceRectangle = null)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
        } 
    }
}
