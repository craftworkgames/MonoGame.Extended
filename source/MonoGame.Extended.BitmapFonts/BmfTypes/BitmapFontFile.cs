// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Represents the contents of a binary BMFont file.  This class cannot be inherited.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
public sealed class BitmapFontFile
{
    private BmfCharsBlock[] _characters;
    private BmfKerningPairsBlock[] _kernings;
    private string[] _pages;

    /// <summary>
    /// Gets the header of the BMFont file.
    /// </summary>
    public BmfHeader Header { get; }

    /// <summary>
    /// Gets the common block of the BMFont file.
    /// </summary>
    public BmfCommonBlock Common { get; }

    /// <summary>
    /// Gets the info block of the BMFont file.
    /// </summary>
    public BmfInfoBlock Info { get; }

    /// <summary>
    /// Gets the name of the font.
    /// </summary>
    public string FontName { get; }

    /// <summary>
    /// Gets the pages (textures) used by the BMFont file.
    /// </summary>
    public ReadOnlySpan<string> Pages => _pages;

    /// <summary>
    /// Gets the characters defined in the BMFont file.
    /// </summary>
    public ReadOnlySpan<BmfCharsBlock> Characters => _characters;

    /// <summary>
    /// Gets the kerning pairs defined in the BMFont file.
    /// </summary>
    public ReadOnlySpan<BmfKerningPairsBlock> Kernings => _kernings;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapFontFile"/> class.
    /// </summary>
    /// <param name="header">The header of the BMFont file.</param>
    /// <param name="common">The common block of the BMFont file.</param>
    /// <param name="info">The info block of the BMFont file.</param>
    /// <param name="fontName">The name of the font.</param>
    /// <param name="pages">The pages (textures) used by the BMFont file.</param>
    /// <param name="characters">The characters defined in the BMFont file.</param>
    /// <param name="kernings">The kerning pairs defined in the BMFont file.</param>
    public BitmapFontFile(BmfHeader header, BmfCommonBlock common, BmfInfoBlock info, string fontName, string[] pages, BmfCharsBlock[] characters, BmfKerningPairsBlock[] kernings) =>
        (Header, Common, Info, FontName, _pages, _characters, _kernings) = (header, common, info, fontName, pages, characters, kernings);
}
