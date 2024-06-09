// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Defines the structure of the kerning pairs block in a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
[StructLayout(LayoutKind.Explicit)]
public struct BmfKerningPairsBlock
{
    /// <summary>
    /// The size of the BmfKerningPairsBlock structure in bytes.
    /// </summary>
    public const int StructSize = 10;

    /// <summary>
    /// The first character ID in the kerning pair.
    /// </summary>
    [FieldOffset(0)]
    public uint First;

    /// <summary>
    /// The second character ID in the kerning pair.
    /// </summary>
    [FieldOffset(4)]
    public uint Second;

    /// <summary>
    /// The amount to adjust the spacing between the first and second characters.
    /// </summary>
    [FieldOffset(8)]
    public short Amount;
}
