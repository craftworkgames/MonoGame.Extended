using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabelStyle : GuiControlStyle
    {
        public GuiLabelStyle(BitmapFont font)
            : this(font, Color.White)
        {
        }

        public GuiLabelStyle(BitmapFont font, Color color)
        {
            Font = font;
            Color = color;
        }

        public BitmapFont Font { get; set; }
        public Color Color { get; set; }
    }
}