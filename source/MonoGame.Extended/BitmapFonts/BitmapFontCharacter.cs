// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.BitmapFonts;

/// <summary>
/// Represents a character in a bitmap font. This class cannot be inherited.
/// </summary>
public sealed class BitmapFontCharacter
{
    /// <summary>
    /// Gets the character code.
    /// </summary>
    public int Character { get; }

    /// <summary>
    /// Gets the texture region that contains the character's image.
    /// </summary>
    public Texture2DRegion TextureRegion { get; }

    /// <summary>
    /// Gets the horizontal offset for rendering the character.
    /// </summary>
    public int XOffset { get; }

    /// <summary>
    /// Gets the vertical offset for rendering the character.
    /// </summary>
    public int YOffset { get; }

    /// <summary>
    /// Gets the horizontal advance value for rendering the next character.
    /// </summary>
    public int XAdvance { get; }

    /// <summary>
    /// Gets the dictionary of kerning values for pairs of characters.
    /// </summary>
    public Dictionary<int, int> Kernings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapFontCharacter"/> class.
    /// </summary>
    /// <param name="character">The character code.</param>
    /// <param name="textureRegion">The texture region that contains the character's image.</param>
    /// <param name="xOffset">The horizontal offset for rendering the character.</param>
    /// <param name="yOffset">The vertical offset for rendering the character.</param>
    /// <param name="xAdvance">The horizontal advance value for rendering the next character.</param>
    public BitmapFontCharacter(int character, Texture2DRegion textureRegion, int xOffset, int yOffset, int xAdvance)
    {
        Character = character;
        TextureRegion = textureRegion;
        XOffset = xOffset;
        YOffset = yOffset;
        XAdvance = xAdvance;
        Kernings = new Dictionary<int, int>();
    }
}
