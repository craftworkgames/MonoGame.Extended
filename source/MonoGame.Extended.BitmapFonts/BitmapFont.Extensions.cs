// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts;

/// <summary>
/// Provides extension methods for <see cref="BitmapFont"/>.
/// </summary>
public static class BitmapFontExtensions
{
    /// <summary>
    /// Measures the size of the given text when rendered with the specified font.
    /// </summary>
    /// <param name="font">The <see cref="BitmapFont"/> used to render the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <returns>The size of the rendered text.</returns>
    public static SizeF MeasureString(this BitmapFont font, string text)
    {
        SizeF size = new SizeF(0, font.LineHeight);
        SizeF offset = new SizeF(0, 0);
        bool firstCharacterOfLine = true;
        MeasureStringInternal(font, text, ref size, ref offset, ref firstCharacterOfLine);
        return size;
    }

    /// <summary>
    /// Measures the size of the given text when rendered with the specified font.
    /// </summary>
    /// <param name="font">The <see cref="BitmapFont"/> used to render the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <returns>The size of the rendered text.</returns>
    public static SizeF MeasureString(this BitmapFont font, StringBuilder text)
    {
        SizeF size = new SizeF(0, font.LineHeight);
        SizeF offset = new SizeF(0, 0);
        bool firstCharacterOfLine = true;

        foreach (ReadOnlyMemory<char> chunk in text.GetChunks())
        {
            MeasureStringInternal(font, chunk.Span, ref size, ref offset, ref firstCharacterOfLine);
        }

        return size;
    }

    /// <summary>
    /// Gets the rectangular bounds of the specified text when rendered with the specified font.
    /// </summary>
    /// <param name="font">The <see cref="BitmapFont"/> used to render the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="location">The top-left corner of the bounding rectangle.</param>
    /// <returns>The rectangular bounds of the rendered text.</returns>
    public static RectangleF GetStringBounds(this BitmapFont font, string text, Vector2 location)
    {
        SizeF size = font.MeasureString(text);
        return new RectangleF(location, size);
    }

    /// <summary>
    /// Gets the rectangular bounds of the specified text when rendered with the specified font.
    /// </summary>
    /// <param name="font">The <see cref="BitmapFont"/> used to render the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="location">The top-left corner of the bounding rectangle.</param>
    /// <returns>The rectangular bounds of the rendered text.</returns>
    public static RectangleF GetStringBounds(this BitmapFont font, StringBuilder text, Vector2 location)
    {
        SizeF size = font.MeasureString(text);
        return new RectangleF(location, size);
    }

    private static void MeasureStringInternal(this BitmapFont font, ReadOnlySpan<char> text, ref SizeF size, ref SizeF offset, ref bool firstCharacterOfLine)
    {
        if (text.Length == 0)
        {
            return;
        }

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '\r')
            {
                continue;
            }

            if (c == '\n')
            {
                offset.Width = 0;
                offset.Height += font.LineHeight;
                firstCharacterOfLine = true;
                continue;
            }

            if (!font.Characters.TryGetValue(c, out BitmapFontCharacter? character))
            {
                continue;
            }

            if (firstCharacterOfLine)
            {
                offset.Width = Math.Max(character.XOffset, 0);
                firstCharacterOfLine = false;
            }
            else
            {
                offset.Width += character.XOffset;
            }

            offset.Width += character.TextureRegion.Width;

            float proposedWidth = offset.Width + character.XAdvance;
            if (proposedWidth > size.Width)
            {
                size.Width = proposedWidth;
            }

            offset.Width += character.XAdvance;

            if (character.TextureRegion.Height > size.Height)
            {
                size.Height = character.TextureRegion.Height;
            }
        }

        size.Height += offset.Height;
    }
}
