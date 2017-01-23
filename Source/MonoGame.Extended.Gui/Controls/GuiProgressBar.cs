using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiProgressBar : GuiControl
    {
        public GuiProgressBar()
        {
        }

        public GuiProgressBar(TextureRegion2D textureRegion)
            : base(textureRegion)
        {
        }

        public float Progress { get; set; }
    }
}