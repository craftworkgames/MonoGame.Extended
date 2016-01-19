using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Styles
{
    //public class GuiTextStyle : GuiControlStyle
    //{
    //    private readonly BitmapFont _font;

    //    public GuiTextStyle(BitmapFont font, string text)
    //    {
    //        _font = font;
    //        Color = Color.White;
    //    }

    //    public Color Color { get; set; }
    //    public string Text { get; set; }

    //    public override IShapeF BoundingShape
    //    {
    //        get { return _font.GetStringRectangle(Text, Vector2.Zero).ToRectangleF(); }
    //    }

    //    public override void Draw(GuiControl control, SpriteBatch spriteBatch)
    //    {
    //        spriteBatch.DrawString(_font, Text, control.Position, Color);
    //    }
    //}
}