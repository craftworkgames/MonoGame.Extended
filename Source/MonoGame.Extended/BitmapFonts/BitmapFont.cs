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
        public static bool UseKernings { get; set; } = true;

        public BitmapFontRegion GetCharacterRegion(int character)
        {
            BitmapFontRegion region;
            return _characterMap.TryGetValue(character, out region) ? region : null;
        }

        public static int GetUnicodeCodePoint(string s, int index)
        {
            return char.IsLowSurrogate(s, index) ? 0 : char.ConvertToUtf32(s, index);
        }

        public Size2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Size2.Empty;

            var stringRectangle = GetStringRectangle(text, Point.Zero);
            return new Size2(stringRectangle.Width, stringRectangle.Height);
        }

        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            return GetStringRectangle(text, position.ToPoint());
        }

        public Rectangle GetStringRectangle(string text, Point position)
        {
            var glyphs = GetGlyphs(text, position.ToVector2());
            var rectangle = new Rectangle(position.X, position.Y, 0, LineHeight);

            foreach (var glyph in glyphs)
            {
                if (glyph.FontRegion != null)
                {
                    var right = glyph.Position.X + glyph.FontRegion.Width;

                    if (right > rectangle.Right)
                        rectangle.Width = (int)(right - rectangle.Left);
                }

                if (glyph.Character == '\n')
                    rectangle.Height += LineHeight;
            }

            return rectangle;
        }

        public BitmapFontGlyph[] GetGlyphs(string text, Vector2 position)
        {
            var glyphs = new BitmapFontGlyph[text.Length];
            var dx = position.X;
            var dy = position.Y;
            var previousGlyph = (BitmapFontGlyph?) null;

            for (var i = 0; i < text.Length; i++)
            {
                var character = GetUnicodeCodePoint(text, i);
                glyphs[i] = new BitmapFontGlyph
                {
                    Character = character,
                    FontRegion = GetCharacterRegion(character),
                    Position = new Vector2(dx, dy)
                };

                if (glyphs[i].FontRegion != null)
                {
                    glyphs[i].Position.X += glyphs[i].FontRegion.XOffset;
                    glyphs[i].Position.Y += glyphs[i].FontRegion.YOffset;
                    dx += glyphs[i].FontRegion.XAdvance + LetterSpacing;
                }

                if (UseKernings && previousGlyph.HasValue && previousGlyph.Value.FontRegion != null)
                {
                    int amount;

                    if (previousGlyph.Value.FontRegion.Kernings.TryGetValue(character, out amount))
                        dx += amount;
                }

                previousGlyph = glyphs[i];

                if (character == '\n')
                {
                    dy += LineHeight;
                    dx = position.X;
                    previousGlyph = null;
                }
            }

            return glyphs;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public struct BitmapFontGlyph
    {
        public int Character;
        public Vector2 Position;
        public BitmapFontRegion FontRegion;
    }
}