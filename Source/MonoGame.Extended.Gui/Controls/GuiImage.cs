using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiImage : GuiControl
    {
        public GuiImage()
        {
        }

        public GuiImage(GuiSkin skin) 
            : base(skin)
        {
        }

        public GuiImage(GuiSkin skin, TextureRegion2D image)
            : base(skin)
        {
            BackgroundRegion = image;
        }
    }
}