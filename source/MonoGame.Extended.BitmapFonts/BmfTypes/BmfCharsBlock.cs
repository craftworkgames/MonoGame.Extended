// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Defines the structure of the chars block in a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
[StructLayout(LayoutKind.Explicit)]
public struct BmfCharsBlock
{
    /// <summary>
    /// The size of the struct in bytes.
    /// </summary>
    public const int StructSize = 20;

    /// <summary>
    /// The character ID.
    /// </summary>
    [FieldOffset(0)]
    public uint ID;

    /// <summary>
    /// The x-coordinate of the character in the texture.
    /// </summary>
    [FieldOffset(4)]
    public ushort X;

    /// <summary>
    /// The y-coordinate of the character in the texture.
    /// </summary>
    [FieldOffset(6)]
    public ushort Y;

    /// <summary>
    /// The width of the character image in the texture.
    /// </summary>
    [FieldOffset(8)]
    public ushort Width;

    /// <summary>
    /// The height of the character image in the texture.
    /// </summary>
    [FieldOffset(10)]
    public ushort Height;

    /// <summary>
    /// The x-offset for the character.
    /// </summary>
    [FieldOffset(12)]
    public short XOffset;

    /// <summary>
    /// The y-offset for the character.
    /// </summary>
    [FieldOffset(14)]
    public short YOffset;

    /// <summary>
    /// The amount to advance the current position after drawing the character.
    /// </summary>
    [FieldOffset(16)]
    public short XAdvance;

    /// <summary>
    /// The texture page where the character image is found.
    /// </summary>
    [FieldOffset(18)]
    public byte Page;

    /// <summary>
    /// The channel where the character image is found.
    /// </summary>
    [FieldOffset(19)]
    public byte Chnl;
}
