using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.BitmapFonts
{
    public static class BitmapFontExtensions
    {
        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position, Color color, int wrapWidth = int.MaxValue, float layerDepth = 0f)
        {
            if (wrapWidth == int.MaxValue)
            {
                DrawString(spriteBatch, bitmapFont, text, position, color, layerDepth);
                return;
            }

            var dx = position.X;
            var dy = position.Y;
            var sentences = text.Split(new[] {'\n'}, StringSplitOptions.None);

            foreach (var sentence in sentences)
            {
                var words = sentence.Split(new[] { ' ' }, StringSplitOptions.None);

                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var size = bitmapFont.GetStringRectangle(word, Vector2.Zero);

                    if (i != 0 && dx + size.Width >= wrapWidth)
                    {
                        dy += bitmapFont.LineHeight;
                        dx = position.X;
                    }

                    DrawString(spriteBatch, bitmapFont, word, new Vector2(dx, dy), color, layerDepth);
                    dx += size.Width + bitmapFont.GetCharacterRegion(' ').XAdvance;
                }

                dx = position.X;
                dy += bitmapFont.LineHeight;
            }
        }

        // this overload is no longer public because the method signature conflicts with the other one,
        // instead the other one calls this one.
        private static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position, Color color, float layerDepth)
        {
            var dx = position.X;
            var dy = position.Y;

            foreach (var character in BitmapFont.GetUnicodeCodePoints(text))
            {
                var fontRegion = bitmapFont.GetCharacterRegion(character);

                if (fontRegion != null)
                {
                    var charPosition = new Vector2(dx + fontRegion.XOffset, dy + fontRegion.YOffset);

                    spriteBatch.Draw(fontRegion.TextureRegion, charPosition, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
                    dx += fontRegion.XAdvance;
                }

                if (character == '\n')
                {
                    dy += bitmapFont.LineHeight;
                    dx = position.X;
                }
            }
        }
    }
}