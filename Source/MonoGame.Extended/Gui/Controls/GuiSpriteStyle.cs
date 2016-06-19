using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiSpriteStyle
    {
        public GuiSpriteStyle()
        {
            Color = Color.White;
        }

        public string TextureRegion { get; set; }
        public Color Color { get; set; }
    }
}