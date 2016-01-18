using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui
{
    public class GuiTextStyle : GuiControlStyle
    {
        private readonly BitmapFont _font;

        public GuiTextStyle(BitmapFont font, string text)
        {
            _font = font;
            Color = Color.White;
        }

        public Color Color { get; set; }
        public string Text { get; set; }

        public override IShapeF BoundingShape
        {
            get { return _font.GetStringRectangle(Text, Vector2.Zero).ToRectangleF(); }
        }

        public override void Draw(GuiDrawableControl control, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, Text, control.Position, Color);
        }
    }

    public class GuiLabel : GuiDrawableControl
    {
        public GuiLabel(GuiTextStyle textStyle)
        {
            TextStyle = textStyle;
        }

        public GuiTextStyle TextStyle { get; set; }
        public string Text { get; set; }

        public override GuiControlStyle CurrentStyle
        {
            get { return TextStyle; }
        }
    }
}