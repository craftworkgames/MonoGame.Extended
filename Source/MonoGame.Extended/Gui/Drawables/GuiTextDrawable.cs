using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Gui.Drawables
{
    public interface IGuiDrawable
    {
        Size DesiredSize { get; }
        void Draw(SpriteBatch spriteBatch, Rectangle rectangle);
    }

    public class GuiTextDrawable : IGuiDrawable
    {
        public GuiTextDrawable(BitmapFont font, string text, Color color)
        {
            Font = font;
            Text = text;
            Color = color;
        }

        public BitmapFont Font { get; private set; }
        public string Text { get; private set; }
        public Color Color { get; private set; }

        public Size DesiredSize
        {
            get { return Font.GetSize(Text); }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            spriteBatch.DrawString(Font, Text, rectangle.Location.ToVector2(), Color);
        }
    }
}