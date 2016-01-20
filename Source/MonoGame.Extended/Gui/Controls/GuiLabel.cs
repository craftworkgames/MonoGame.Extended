using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Styles;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiLabelStyle : GuiControlStyle
    {
        public GuiLabelStyle(BitmapFont font)
        {
            Font = font;
        }

        public BitmapFont Font { get; private set; }
    }

    public class GuiTextDrawable : IGuiDrawable
    {
        public GuiTextDrawable(BitmapFont font, string text)
        {
            Font = font;
            Text = text;
        }

        public BitmapFont Font { get; private set; }
        public string Text { get; private set; }

        public Size Size
        {
            get
            {
                var stringRectangle = Font.GetStringRectangle(Text, Vector2.Zero);
                return new Size(stringRectangle.Width, stringRectangle.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle bounds)
        {
            throw new System.NotImplementedException();
        }
    }

    public class GuiLabel : GuiControl
    {
        public GuiLabel(GuiLabelStyle style)
        {
            Style = style;
        }

        public GuiLabelStyle Style { get; set; }
        public string Text { get; set; }

        protected override IGuiDrawable GetCurrentDrawable()
        {
            return new GuiTextDrawable(Style.Font, Text);
        }
    }
}