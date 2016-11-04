using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont
    {
        internal BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight)
        {
            _characterMap = regions.ToDictionary(r => r.Character);

            Name = name;
            LineHeight = lineHeight;
        }

        private readonly Dictionary<int, BitmapFontRegion> _characterMap;

        public string Name { get; }
        public int LineHeight { get; private set; }
        public int LetterSpacing { get; set; } = 0;

        public BitmapFontRegion GetCharacterRegion(int character)
        {
            BitmapFontRegion region;
            return _characterMap.TryGetValue(character, out region) ? region : null;
        }

        internal static IEnumerable<int> GetUnicodeCodePoints(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                for (int i = 0; i < s.Length; i += 1)
                {
                    if (char.IsLowSurrogate(s, i))
                        continue;

                    yield return char.ConvertToUtf32(s, i);
                }
            }
        }

        public Size GetSize(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var totalWidth = 0;
            var lineWidth = 0;
            var totalHeight = 0;
            var lineHeight = 0;

            const int newlineCodePoint = '\n';

            var codePoints = GetUnicodeCodePoints(text).ToArray();

            for (int i = 0, l = codePoints.Length; i < l; i++)
            {
                BitmapFontRegion fontRegion;
                var character = codePoints[i];
                var nextCharacter = character;

                if (i < l - 1) nextCharacter = codePoints[i + 1];

                if (_characterMap.TryGetValue(character, out fontRegion))
                {
                    // Add LetterSpacing unless end of string or next character is not in _characterMap
                    if (i != text.Length - 1 && _characterMap.ContainsKey(nextCharacter))
                        lineWidth += fontRegion.XAdvance + LetterSpacing;
                    else
                        lineWidth += fontRegion.XOffset + fontRegion.Width;

                    if (fontRegion.Height + fontRegion.YOffset > lineHeight)
                        lineHeight = fontRegion.Height + fontRegion.YOffset;
                }

                if (character == newlineCodePoint)
                {
                    totalHeight += lineHeight;
                    if (totalWidth < lineWidth) totalWidth = lineWidth;

                    lineHeight = 0;
                    lineWidth = 0;
                }
            }

            if (totalWidth == 0)
                totalWidth = lineWidth;
            totalHeight += lineHeight;

            return new Size(totalWidth, totalHeight);
        }

        public Size MeasureString(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var size = GetSize(text);
            return size;
        }

        public Size MeasureString(StringBuilder stringBuilder)
        {
            if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));

            return MeasureString(stringBuilder.ToString());
        }

        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            var size = GetSize(text);
            var p = position.ToPoint();
            return new Rectangle(p.X, p.Y, size.Width, size.Height);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
