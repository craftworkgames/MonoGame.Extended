// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.BitmapFonts;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.BitmapFonts;

public sealed class BitmapFont
{
    private readonly Dictionary<int, BitmapFontCharacter> _characters;
    public string Face { get; }
    public int Size { get; }
    public int LineHeight { get; }

    public int LetterSpacing { get; set; }

    public bool UseKernings { get; set; } = true;

    public BitmapFont(string face, int size, int lineHeight, IEnumerable<BitmapFontCharacter> characters)
    {
        Face = face;
        Size = size;
        LineHeight = lineHeight;
        _characters = new Dictionary<int, BitmapFontCharacter>();

        foreach (BitmapFontCharacter character in characters)
        {
            _characters.Add(character.Character, character);
        }
    }

    public BitmapFontCharacter GetCharacter(int character) => _characters.TryGetValue(character, out BitmapFontCharacter fontCharacter) ? fontCharacter : null;/*  _characters[character];*/
    public bool TryGetCharacter(int character, out BitmapFontCharacter value) => _characters.TryGetValue(character, out value);

    public SizeF MeasureString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return SizeF.Empty;

        var stringRectangle = GetStringRectangle(text);
        return new SizeF(stringRectangle.Width, stringRectangle.Height);
    }

    public SizeF MeasureString(StringBuilder text)
    {
        if (text == null || text.Length == 0)
            return SizeF.Empty;

        var stringRectangle = GetStringRectangle(text);
        return new SizeF(stringRectangle.Width, stringRectangle.Height);
    }

    public RectangleF GetStringRectangle(string text)
    {
        return GetStringRectangle(text, Vector2.Zero);
    }

    public RectangleF GetStringRectangle(string text, Vector2 position)
    {
        var glyphs = GetGlyphs(text, position);
        var rectangle = new RectangleF(position.X, position.Y, 0, LineHeight);

        foreach (var glyph in glyphs)
        {
            if (glyph.Character != null)
            {
                var right = glyph.Position.X + glyph.Character.TextureRegion.Width;

                if (right > rectangle.Right)
                    rectangle.Width = (int)(right - rectangle.Left);
            }

            if (glyph.CharacterID == '\n')
                rectangle.Height += LineHeight;
        }

        return rectangle;
    }

    public RectangleF GetStringRectangle(StringBuilder text, Vector2? position = null)
    {
        var position1 = position ?? new Vector2();
        var glyphs = GetGlyphs(text, position1);
        var rectangle = new RectangleF(position1.X, position1.Y, 0, LineHeight);

        foreach (var glyph in glyphs)
        {
            if (glyph.Character != null)
            {
                var right = glyph.Position.X + glyph.Character.TextureRegion.Width;

                if (right > rectangle.Right)
                    rectangle.Width = (int)(right - rectangle.Left);
            }

            if (glyph.CharacterID == '\n')
                rectangle.Height += LineHeight;
        }

        return rectangle;
    }

    public struct BitmapFontGlyph
    {
        public int CharacterID;
        public Vector2 Position;
        public BitmapFontCharacter Character;
    }
    public StringGlyphEnumerable GetGlyphs(string text, Vector2? position = null)
    {
        return new StringGlyphEnumerable(this, text, position);
    }

    public StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text, Vector2? position)
    {
        return new StringBuilderGlyphEnumerable(this, text, position);
    }

    public struct StringGlyphEnumerable : IEnumerable<BitmapFontGlyph>
    {
        private readonly StringGlyphEnumerator _enumerator;

        public StringGlyphEnumerable(BitmapFont font, string text, Vector2? position)
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
        private readonly Vector2 _position;
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

        public StringGlyphEnumerator(BitmapFont font, string text, Vector2? position)
        {
            _font = font;
            _text = text;
            _index = -1;
            _position = position ?? new Vector2();
            _positionDelta = new Vector2();
            _currentGlyph = new BitmapFontGlyph();
            _previousGlyph = null;
        }

        public bool MoveNext()
        {
            if (++_index >= _text.Length)
                return false;

            var character = GetUnicodeCodePoint(_text, ref _index);
            _currentGlyph.CharacterID = character;
            _font.TryGetCharacter(character, out _currentGlyph.Character);
            _currentGlyph.Position = _position + _positionDelta;

            if (_currentGlyph.Character != null)
            {
                _currentGlyph.Position.X += _currentGlyph.Character.XOffset;
                _currentGlyph.Position.Y += _currentGlyph.Character.YOffset;
                _positionDelta.X += _currentGlyph.Character.XAdvance + _font.LetterSpacing;
            }

            if (_font.UseKernings && _previousGlyph?.Character != null)
            {
                if (_previousGlyph.Value.Character.Kernings.TryGetValue(character, out var amount))
                {
                    _positionDelta.X += amount;
                    _currentGlyph.Position.X += amount;
                }
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
            _positionDelta = new Vector2();
            _index = -1;
            _previousGlyph = null;
        }
    }

    public struct StringBuilderGlyphEnumerable : IEnumerable<BitmapFontGlyph>
    {
        private readonly StringBuilderGlyphEnumerator _enumerator;

        public StringBuilderGlyphEnumerable(BitmapFont font, StringBuilder text, Vector2? position)
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
        private readonly Vector2 _position;
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

        public StringBuilderGlyphEnumerator(BitmapFont font, StringBuilder text, Vector2? position)
        {
            _font = font;
            _text = text;
            _index = -1;
            _position = position ?? new Vector2();
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
                CharacterID = character,
                Character = _font.GetCharacter(character),
                Position = _position + _positionDelta
            };

            if (_currentGlyph.Character != null)
            {
                _currentGlyph.Position.X += _currentGlyph.Character.XOffset;
                _currentGlyph.Position.Y += _currentGlyph.Character.YOffset;
                _positionDelta.X += _currentGlyph.Character.XAdvance + _font.LetterSpacing;
            }

            if (_font.UseKernings && _previousGlyph.HasValue && _previousGlyph.Value.Character != null)
            {
                int amount;
                if (_previousGlyph.Value.Character.Kernings.TryGetValue(character, out amount))
                {
                    _positionDelta.X += amount;
                    _currentGlyph.Position.X += amount;
                }
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
            _positionDelta = new Vector2();
            _index = -1;
            _previousGlyph = null;
        }
    }



    /// <inheritdoc/>
    public override string ToString() => $"{Face}";

    public static BitmapFont FromFile(GraphicsDevice graphicsDevice, string path)
    {
        using Stream stream = TitleContainer.OpenStream(path);
        return FromStream(graphicsDevice, stream, path);
    }

    [Obsolete("Use the FromStream() overload that takes an explicit name.")]
    public static BitmapFont FromStream(GraphicsDevice graphicsDevice, FileStream stream)
    {
        return FromStream(graphicsDevice, stream, stream.Name);
    }

    public static BitmapFont FromStream(GraphicsDevice graphicsDevice, Stream stream, string name)
    {
        var bmfFile = BitmapFontFileReader.Read(stream, name);

        //  Load page textures
        Dictionary<string, Texture2D> pages = new Dictionary<string, Texture2D>();
        for (int i = 0; i < bmfFile.Pages.Count; i++)
        {
            if (!pages.ContainsKey(bmfFile.Pages[i]))
            {
                string texturePath = Path.Combine(Path.GetDirectoryName(bmfFile.Path), bmfFile.Pages[i]);
                using (Stream textureStream = TitleContainer.OpenStream(texturePath))
                {
                    Texture2D texture = Texture2D.FromStream(graphicsDevice, textureStream);
                    pages.Add(bmfFile.Pages[i], texture);
                }
            }
        }

        //  Load Characters
        Dictionary<int, BitmapFontCharacter> characters = new Dictionary<int, BitmapFontCharacter>();
        for (int i = 0; i < bmfFile.Characters.Count; i++)
        {
            var charBlock = bmfFile.Characters[i];
            Texture2D texture = pages[bmfFile.Pages[charBlock.Page]];
            Texture2DRegion region = new Texture2DRegion(texture, charBlock.X, charBlock.Y, charBlock.Width, charBlock.Height);
            BitmapFontCharacter character = new BitmapFontCharacter((int)charBlock.ID, region, charBlock.XOffset, charBlock.YOffset, charBlock.XAdvance);
            characters.Add(character.Character, character);
        }

        //  Load kernings
        for (int i = 0; i < bmfFile.Kernings.Count; i++)
        {
            var kerningBlock = bmfFile.Kernings[i];
            if (characters.TryGetValue((int)kerningBlock.First, out BitmapFontCharacter character))
            {
                character.Kernings.Add((int)kerningBlock.Second, kerningBlock.Amount);
            }
        }

        return new BitmapFont(bmfFile.FontName, bmfFile.Info.FontSize, bmfFile.Common.LineHeight, characters.Values);
    }
}
