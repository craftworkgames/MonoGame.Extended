using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont
    {
        private readonly Dictionary<int, BitmapFontRegion> _characterMap;

        public BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight)
        {
            _characterMap = regions.ToDictionary(r => r.Character);

            Name = name;
            LineHeight = lineHeight;
        }

        public string Name { get; }
        public int LineHeight { get; }
        public int LetterSpacing { get; set; } = 0;

        public BitmapFontRegion GetCharacterRegion(int character)
        {
            BitmapFontRegion region;
            return _characterMap.TryGetValue(character, out region) ? region : null;
        }

        public static int GetUnicodeCodePoint(string s, int index)
        {
            return char.IsLowSurrogate(s, index) ? 0 : char.ConvertToUtf32(s, index);
        }

        public Size MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            var stringRectangle = GetStringRectangle(text, Point.Zero);
            return new Size(stringRectangle.Width, stringRectangle.Height);
        }

        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            return GetStringRectangle(text, position.ToPoint());
        }

        public Rectangle GetStringRectangle(string text, Point position)
        {
            var dx = position.X;
            var dy = position.Y;
            var rectangle = new Rectangle(dx, dy, 0, LineHeight);

            for (var i = 0; i < text.Length; i++)
            {
                var character = GetUnicodeCodePoint(text, i);
                var fontRegion = GetCharacterRegion(character);

                if (fontRegion != null)
                {
                    var characterPosition = new Point(dx + fontRegion.XOffset, dy + fontRegion.YOffset);
                    var right = characterPosition.X + fontRegion.Width;

                    if (right > rectangle.Right)
                        rectangle.Width = right - rectangle.Left;

                    dx += fontRegion.XAdvance + LetterSpacing;
                }

                if (character == '\n')
                {
                    rectangle.Height += LineHeight;
                    dx = position.X;
                }
            }

            return rectangle;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}