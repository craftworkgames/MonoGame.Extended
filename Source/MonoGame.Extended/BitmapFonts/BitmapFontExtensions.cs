using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using System.Linq;

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
                    dx += size.Width;

                    var spaceCharRegion = bitmapFont.GetCharacterRegion(' ');
                    if (i != words.Length - 1)
                        dx += spaceCharRegion.XAdvance + bitmapFont.LetterSpacing;
                    else
                        dx += spaceCharRegion.XOffset + spaceCharRegion.Width;
                }

                dx = position.X;
                dy += bitmapFont.LineHeight;
            }
        }

        // this overload is no longer public because the method signature conflicts with the other one,
        // instead the other one calls this one.
        internal static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position, Color color, float layerDepth)
        {
            var dx = position.X;
            var dy = position.Y;
            var codePoints = BitmapFont.GetUnicodeCodePoints(text).ToArray();

            for (int i = 0, l = codePoints.Length; i < l; i++)
            {
                var character = codePoints[i];
                var fontRegion = bitmapFont.GetCharacterRegion(character);

                if (fontRegion != null)
                {
                    var charPosition = new Vector2(dx + fontRegion.XOffset, dy + fontRegion.YOffset);

                    spriteBatch.Draw(fontRegion.TextureRegion, charPosition, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);

                    if (i != text.Length - 1)
                        dx += fontRegion.XAdvance + bitmapFont.LetterSpacing;
                    else
                        dx += fontRegion.XOffset + fontRegion.Width;
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