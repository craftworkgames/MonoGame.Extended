using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;

namespace Demo.Screens.Screens
{
    public class MenuItem
    {
        public MenuItem(BitmapFont font, string text)
        {
            Text = text;
            Font = font;
            Color = Color.White;
        }

        public BitmapFont Font { get; }
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public RectangleF BoundingRectangle => new RectangleF(Position, Font.MeasureString(Text));
        public Action Action { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position, Color);
        }

    }
}