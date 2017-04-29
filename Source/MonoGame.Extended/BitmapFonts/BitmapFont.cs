using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont
    {
        private readonly Dictionary<int, BitmapFontRegion> _characterMap = new Dictionary<int, BitmapFontRegion>();

        public BitmapFont(string name, IEnumerable<BitmapFontRegion> regions, int lineHeight)
        {
            foreach (var region in regions)
                _characterMap.Add(region.Character, region);

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

        public Size2 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Size2.Empty;

            var stringRectangle = GetStringRectangle(text);
            return new Size2(stringRectangle.Width, stringRectangle.Height);
        }

        public Size2 MeasureString(StringBuilder text)
        {
            if (text == null || text.Length == 0)
                return Size2.Empty;

            var stringRectangle = GetStringRectangle(text);
            return new Size2(stringRectangle.Width, stringRectangle.Height);
        }

        public RectangleF GetStringRectangle(string text, Point2? position = null)
        {
            var position1 = position ?? new Point2();
            var glyphs = GetGlyphs(text, position1);
            var rectangle = new RectangleF(position1.X, position1.Y, 0, LineHeight);

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

        public RectangleF GetStringRectangle(StringBuilder text, Point2? position = null)
        {
            var position1 = position ?? new Point2();
            var glyphs = GetGlyphs(text, position1);
            var rectangle = new RectangleF(position1.X, position1.Y, 0, LineHeight);

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

        public StringGlyphEnumerable GetGlyphs(string text, Point2? position = null)
        {
            return new StringGlyphEnumerable(this, text, position);
        }

        public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text, Point2? position)
        {
            return new StringBuilderGlyphEnumerable(this, text, position);
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public struct StringGlyphEnumerable : IEnumerable<BitmapFontGlyph>
        {
            private readonly StringGlyphEnumerator _enumerator;

            public StringGlyphEnumerable(BitmapFont font, string text, Point2? position)
            {
                _enumerator = new StringGlyphEnumerator(font, text, position);
            }

            public StringGlyphEnumerator GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _enumerator;
            }
        }

        public struct StringGlyphEnumerator : IEnumerator<BitmapFontGlyph>
        {
            private readonly BitmapFont _font;
            private readonly string _text;
            private int _index;
            private readonly Point2 _position;
            private Vector2 _positionDelta;
            private BitmapFontGlyph _currentGlyph;
            private BitmapFontGlyph? _previousGlyph;

            object IEnumerator.Current
            {
                get
                {
                    // casting a struct to object will box it, behaviour we want to avoid...
                    throw new InvalidOperationException();
                }
            }

            public BitmapFontGlyph Current => _currentGlyph;

            public StringGlyphEnumerator(BitmapFont font, string text, Point2? position)
            {
                _font = font;
                _text = text;
                _index = -1;
                _position = position ?? new Point2();
                _positionDelta = new Vector2();
                _currentGlyph = new BitmapFontGlyph();
                _previousGlyph = null;
            }

            public bool MoveNext()
            {
                if (++_index >= _text.Length)
                    return false;

                var character = GetUnicodeCodePoint(_text, ref _index);
                _currentGlyph.Character = character;
                _currentGlyph.FontRegion = _font.GetCharacterRegion(character);
                _currentGlyph.Position = _position + _positionDelta;

                if (_currentGlyph.FontRegion != null)
                {
                    _currentGlyph.Position.X += _currentGlyph.FontRegion.XOffset;
                    _currentGlyph.Position.Y += _currentGlyph.FontRegion.YOffset;
                    _positionDelta.X += _currentGlyph.FontRegion.XAdvance + _font.LetterSpacing;
                }

                if (UseKernings && _previousGlyph.HasValue && _previousGlyph.Value.FontRegion != null)
                {
                    int amount;
                    if (_previousGlyph.Value.FontRegion.Kernings.TryGetValue(character, out amount))
                        _positionDelta.X += amount;
                }

                _previousGlyph = _currentGlyph;

                if (character != '\n')
                    return true;

                _positionDelta.Y += _font.LineHeight;
                _positionDelta.X = 0;
                _previousGlyph = null;

                return true;
            }

            private static int GetUnicodeCodePoint(string text, ref int index)
            {
                return char.IsHighSurrogate(text[index]) && ++index < text.Length
                    ? char.ConvertToUtf32(text[index - 1], text[index])
                    : text[index];
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
                _positionDelta = new Point2();
                _index = -1;
                _previousGlyph = null;
            }
        }

        public struct StringBuilderGlyphEnumerable : IEnumerable<BitmapFontGlyph>
        {
            private readonly StringBuilderGlyphEnumerator _enumerator;

            public StringBuilderGlyphEnumerable(BitmapFont font, StringBuilder text, Point2? position)
            {
                _enumerator = new StringBuilderGlyphEnumerator(font, text, position);
            }

            public StringBuilderGlyphEnumerator GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _enumerator;
            }
        }

        public struct StringBuilderGlyphEnumerator : IEnumerator<BitmapFontGlyph>
        {
            private readonly BitmapFont _font;
            private readonly StringBuilder _text;
            private int _index;
            private Point2 _position;
            private Vector2 _positionDelta;
            private BitmapFontGlyph _currentGlyph;
            private BitmapFontGlyph? _previousGlyph;

            object IEnumerator.Current
            {
                get
                {
                    // casting a struct to object will box it, behaviour we want to avoid...
                    throw new InvalidOperationException();
                }
            }

            public BitmapFontGlyph Current => _currentGlyph;

            public StringBuilderGlyphEnumerator(BitmapFont font, StringBuilder text, Point2? position)
            {
                _font = font;
                _text = text;
                _index = -1;
                _position = position ?? new Point2();
                _positionDelta = new Vector2();
                _currentGlyph = new BitmapFontGlyph();
                _previousGlyph = null;
            }

            public bool MoveNext()
            {
                if (++_index >= _text.Length)
                    return false;

                var character = GetUnicodeCodePoint(_text, ref _index);
                _currentGlyph = new BitmapFontGlyph
                {
                    Character = character,
                    FontRegion = _font.GetCharacterRegion(character),
                    Position = _position + _positionDelta
                };

                if (_currentGlyph.FontRegion != null)
                {
                    _currentGlyph.Position.X += _currentGlyph.FontRegion.XOffset;
                    _currentGlyph.Position.Y += _currentGlyph.FontRegion.YOffset;
                    _positionDelta.X += _currentGlyph.FontRegion.XAdvance + _font.LetterSpacing;
                }

                if (UseKernings && _previousGlyph.HasValue && _previousGlyph.Value.FontRegion != null)
                {
                    int amount;
                    if (_previousGlyph.Value.FontRegion.Kernings.TryGetValue(character, out amount))
                        _positionDelta.X += amount;
                }

                _previousGlyph = _currentGlyph;

                if (character != '\n')
                    return true;

                _positionDelta.Y += _font.LineHeight;
                _positionDelta.X = _position.X;
                _previousGlyph = null;

                return true;
            }

            private static int GetUnicodeCodePoint(StringBuilder text, ref int index)
            {
                return char.IsHighSurrogate(text[index]) && ++index < text.Length
                    ? char.ConvertToUtf32(text[index - 1], text[index])
                    : text[index];
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
                _positionDelta = new Point2();
                _index = -1;
                _previousGlyph = null;
            }
        }
    }

    public struct BitmapFontGlyph
    {
        public int Character;
        public Vector2 Position;
        public BitmapFontRegion FontRegion;
    }
}