using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiImage : GuiControl
    {
        public GuiImage()
        {
        }
        
        public GuiImage(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }
    }
}