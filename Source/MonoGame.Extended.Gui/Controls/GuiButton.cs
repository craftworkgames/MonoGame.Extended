using MonoGame.Extended.Gui.Keepers;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public GuiButton()
        {
        }

        public GuiButton(TextureRegion2D textureRegion)
        {
            BackgroundRegion = textureRegion;
            Size = BackgroundRegion.Size;
        }
    }
}