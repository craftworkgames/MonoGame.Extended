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

            foreach (var character in text)
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

        // TODO: DrawString with word wrap
        //public void DrawString(this SpriteBatch spriteBatch, string text, int x, int y, int wrapWidth, Color color)
        //{
        //    var dw = 0;
        //    var dx = x;
        //    var dy = y;
        //    var words = text.Split(new [] {' ','\n'}, StringSplitOptions.None);

        //    foreach (var word in words)
        //    {
        //        var size = GetStringRectangle(word, 0, 0);

        //        Draw(spriteBatch, word, dx, dy, color);

        //        if (dw > wrapWidth)
        //        {
        //            dy += _fontFile.Common.LineHeight;
        //            dw = 0;
        //            dx = x;
        //        }
        //        else
        //        {
        //            dx += size.Width + _characterMap[' '].FontCharacter.XAdvance;
        //        }

        //        dw += size.Width;
        //    }
        //}
    }
}