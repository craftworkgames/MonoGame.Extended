using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont 
    {
        public BitmapFont(string name, Texture2D texture, FontFile fontFile)
        {
            Name = name;
            _fontFile = fontFile;
            _characterMap = BuildCharacterMap(fontFile, texture);
        }

        private struct FontRegion
        {
            public FontChar FontChar { get; set; }
            public TextureRegion2D TextureRegion { get; set; }
        }

        private readonly FontFile _fontFile;
        private readonly Dictionary<char, FontRegion> _characterMap;

        private static Dictionary<char, FontRegion> BuildCharacterMap(FontFile fontFile, Texture2D texture)
        {
            var characterMap = new Dictionary<char, FontRegion>();

            foreach (var fontChar in fontFile.Chars)
            {
                var character = (char)fontChar.ID;
                var region = new TextureRegion2D(texture, fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
                var fontRegion = new FontRegion {FontChar = fontChar, TextureRegion = region};
                characterMap.Add(character, fontRegion);
            }

            return characterMap;
        }

        public string Name { get; private set; }
        
        public void Draw(SpriteBatch spriteBatch, string text, int x, int y)
        {
            Draw(spriteBatch, text, x, y, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, string text, int x, int y, Color color)
        {
            var dx = x;
            var dy = y;

            foreach (char character in text)
            {
                FontRegion fontRegion;

                if (_characterMap.TryGetValue(character, out fontRegion))
                {
                    var fontChar = fontRegion.FontChar;
                    var region = fontRegion.TextureRegion;
                    var position = new Vector2(dx + fontChar.XOffset, dy + fontChar.YOffset);

                    //spriteBatch.Draw(region, position, color, Vector2.Zero, 0, Vector2.One);
                    dx += fontChar.XAdvance;
                }

                if (character == '\n')
                {
                    dy += _fontFile.Common.LineHeight;
                    dx = x;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, string text, int x, int y, int wrapWidth)
        {
            Draw(spriteBatch, text, x, y, wrapWidth, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, string text, int x, int y, int wrapWidth, Color color)
        {
            var dw = 0;
            var dx = x;
            var dy = y;
            var words = text.Split(new [] {' ','\n'}, StringSplitOptions.None);

            foreach (var word in words)
            {
                var size = MeasureText(word, 0, 0);

                Draw(spriteBatch, word, dx, dy, color);

                if (dw > wrapWidth)
                {
                    dy += _fontFile.Common.LineHeight;
                    dw = 0;
                    dx = x;
                }
                else
                {
                    dx += size.Width + _characterMap[' '].FontChar.XAdvance;
                }

                dw += size.Width;
            }
        }

        public Rectangle MeasureText(string text, int x, int y)
        {
            var width = 0;
            var height = 0;

            foreach (char c in text)
            {
                FontRegion fontRegion;

                if (_characterMap.TryGetValue(c, out fontRegion))
                {
                    var fc = fontRegion.FontChar;
                    width += fc.XAdvance;

                    if (fc.Height + fc.YOffset > height)
                        height = fc.Height + fc.YOffset;
                }
            }

            return new Rectangle(x, y, width, height);
        }
    }
}
