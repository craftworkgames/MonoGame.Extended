using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabel : GuiControl
    {
        public GuiLabel()
        {
        }

        public GuiLabel(string text)
        {
            Text = text;
        }

        public GuiLabel(string text, TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
            Text = text;
        }
    }
}