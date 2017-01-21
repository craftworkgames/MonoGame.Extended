using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiPanel : GuiControl
    {
        public GuiPanel()
        {
        }

        public GuiPanel(TextureRegion2D textureRegion)
            : base(textureRegion)
        {
        }
    }
}