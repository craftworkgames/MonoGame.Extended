// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Defines the structure of the header of a BMFont binary file.
/// </summary>
/// <remarks>
/// The first three bytes are the file identifier and should be the characters B, M, and F. The fourth byte is the
/// version, which should always be 3.
/// </remarks>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
[StructLayout(LayoutKind.Explicit)]
public struct BmfHeader
{
    /// <summary>
    /// The size of the struct in bytes.
    /// </summary>
    public const int StructSize = 4;

    /// <summary>
    /// The 'B' character in the file identifier.
    /// </summary>
    [FieldOffset(0)]
    public byte B;

    /// <summary>
    /// The 'M' character in the file identifier.
    /// </summary>
    [FieldOffset(1)]
    public byte M;

    /// <summary>
    /// The 'F' character in the file identifier.
    /// </summary>
    [FieldOffset(2)]
    public byte F;

    /// <summary>
    /// The version of the BMFont file format, which should always be 3.
    /// </summary>
    [FieldOffset(3)]
    public byte Version;

    /// <summary>
    /// Gets a value indicating whether the header is valid.
    /// </summary>
    public readonly bool IsValid => B == 0x42 && M == 0x4D && F == 0x46 && Version == 3;
}
