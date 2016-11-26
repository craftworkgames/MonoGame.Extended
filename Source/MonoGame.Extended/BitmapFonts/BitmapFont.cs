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
            if(string.IsNullOrEmpty(text))
                return Size.Empty;

            var totalWidth = 0;
            var totalHeight = LineHeight;
            var currentLineWidth = 0;

            for (var i = 0; i < text.Length; i++)
            {
                var character = GetUnicodeCodePoint(text, i);
                var fontRegion = GetCharacterRegion(character);

                if (fontRegion != null)
                    currentLineWidth += fontRegion.XOffset + fontRegion.XAdvance + LetterSpacing;

                if (character == '\n')
                {
                    totalWidth = CalculateWidth(currentLineWidth, totalWidth);
                    totalHeight += LineHeight;
                    currentLineWidth = 0;
                }
            }

            totalWidth = CalculateWidth(currentLineWidth, totalWidth);
            return new Size(totalWidth, totalHeight);
        }

        private int CalculateWidth(int currentLineWidth, int totalWidth)
        {
            if (currentLineWidth > 0)
                currentLineWidth -= LetterSpacing;

            return totalWidth < currentLineWidth ? currentLineWidth : totalWidth;
        }
        
        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            var size = MeasureString(text);
            var point = position.ToPoint();
            return new Rectangle(point.X, point.Y, size.Width, size.Height);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}