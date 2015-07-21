using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public static class BitmapFontExtensions
    {
        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position, Color color)
        {
            var dx = position.X;
            var dy = position.Y;

            foreach (char character in text)
            {
                var fontRegion = bitmapFont.GetCharacterRegion(character);

                if (fontRegion != null)
                {
                    var fontChar = fontRegion.FontCharacter;
                    var charPosition = new Vector2(dx + fontChar.XOffset, dy + fontChar.YOffset);

                    spriteBatch.Draw(fontRegion.TextureRegion, charPosition, color);
                    dx += fontChar.XAdvance;
                }

                if (character == '\n')
                {
                    dy += bitmapFont.LineHeight;
                    dx = position.X;
                }
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position, Color color, int wrapWidth)
        {
            var dw = 0;
            var dx = position.X;
            var dy = position.Y;
            var words = text.Split(new[] { ' ', '\n' }, StringSplitOptions.None);

            foreach (var word in words)
            {
                var size = bitmapFont.GetStringRectangle(word, Vector2.Zero);

                DrawString(spriteBatch, bitmapFont, word, new Vector2(dx, dy), color);

                if (dw > wrapWidth)
                {
                    dy += bitmapFont.LineHeight;
                    dw = 0;
                    dx = position.X;
                }
                else
                {
                    dx += size.Width + bitmapFont.GetCharacterRegion(' ').FontCharacter.XAdvance;
                }

                dw += size.Width;
            }
        }
    }
}