using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Drawables
{
    public static class GuiTextureRegionDrawableExtensions
    {
        public static GuiTextureRegionDrawable ToGuiDrawable(this Texture2D texture)
        {
            var region = new TextureRegion2D(texture);
            return new GuiTextureRegionDrawable(region);
        }

        public static GuiTextureRegionDrawable ToGuiDrawable(this TextureRegion2D textureRegion)
        {
            return new GuiTextureRegionDrawable(textureRegion);
        }
    }
}