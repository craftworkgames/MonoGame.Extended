using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    internal struct BitmapFontCharacter
    {
        public BitmapFontRegion Region { get; set; }
        public Vector2 Position { get; set; }
    }

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
                for (var i = 0; i < s.Length; i += 1)
                {
                    if (char.IsLowSurrogate(s, i))
                        continue;

                    yield return char.ConvertToUtf32(s, i);
                }
            }
        }

        internal IEnumerable<BitmapFontCharacter> GetCharacterPositions(string text, Vector2 position, Vector2 scale)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var dx = position.X;
            var dy = position.Y;
            var codePoints = GetUnicodeCodePoints(text).ToArray();
            var positionOffset = Vector2.Zero;

            for (int i = 0, l = codePoints.Length; i < l; i++)
            {
                var character = codePoints[i];
                var fontRegion = GetCharacterRegion(character);

                if (fontRegion != null)
                {
                    var charPosition = new Vector2(dx + fontRegion.XOffset, dy + fontRegion.YOffset);
                    var scaledCharPosition = charPosition * new Vector2(scale.X, scale.Y);

                    if (i == 0)
                        positionOffset = scaledCharPosition - charPosition;

                    scaledCharPosition -= positionOffset;

                    yield return new BitmapFontCharacter {Region = fontRegion, Position = scaledCharPosition};

                    if (i != text.Length - 1)
                        dx += fontRegion.XAdvance + LetterSpacing;
                    else
                        dx += fontRegion.XOffset + fontRegion.Width;
                }

                if (character != '\n')
                    continue;

                dy += LineHeight;
                dx = position.X;
            }
        }

        public Size MeasureString(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            //var totalWidth = 0;
            //var lineWidth = 0;
            //var totalHeight = 0;
            //var lineHeight = 0;

            //const int newlineCodePoint = '\n';

            //var codePoints = GetUnicodeCodePoints(text).ToArray();

            //for (int i = 0, l = codePoints.Length; i < l; i++)
            //{
            //    BitmapFontRegion fontRegion;
            //    var character = codePoints[i];
            //    var nextCharacter = character;

            //    if (i < l - 1) nextCharacter = codePoints[i + 1];

            //    if (_characterMap.TryGetValue(character, out fontRegion))
            //    {
            //        // Add LetterSpacing unless end of string or next character is not in _characterMap
            //        if ((i != text.Length - 1) && _characterMap.ContainsKey(nextCharacter))
            //            lineWidth += fontRegion.XAdvance + LetterSpacing;
            //        else
            //            lineWidth += fontRegion.XOffset + fontRegion.Width;

            //        if (fontRegion.Height + fontRegion.YOffset > lineHeight)
            //            lineHeight = fontRegion.Height + fontRegion.YOffset;
            //    }

            //    if (character == newlineCodePoint)
            //    {
            //        totalHeight += lineHeight;

            //        if (totalWidth < lineWidth)
            //            totalWidth = lineWidth;

            //        lineHeight = 0;
            //        lineWidth = 0;
            //    }
            //}

            //if (totalWidth == 0)
            //    totalWidth = lineWidth;

            //totalHeight += lineHeight;

            //return new Size(totalWidth, totalHeight);

            var stringRectangle = GetStringRectangle(text, Vector2.Zero);
            return new Size(stringRectangle.Width, stringRectangle.Height);
        }
        
        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            var left = position.X;
            var top = position.Y;
            var right = position.X;
            var bottom = position.Y;

            foreach (var c in GetCharacterPositions(text, position, Vector2.One))
            {
                var newRight = c.Position.X + c.Region.Width;
                var newBottom = c.Position.Y + c.Region.Height;

                if (newRight > right)
                    right = newRight;
                
                if (newBottom > bottom)
                    bottom = newBottom;
            }

            //var size = MeasureString(text);
            //var point = position.ToPoint();
            //return new Rectangle(point.X, point.Y, size.Width, size.Height);
            return new Rectangle((int) left, (int) top, (int) (right - left), (int) (bottom - top));
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}