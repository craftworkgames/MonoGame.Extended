// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Defines the structure of the common block in a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
[StructLayout(LayoutKind.Explicit)]
public struct BmfCommonBlock
{
    /// <summary>
    /// The size of the struct in bytes.
    /// </summary>
    public const int StructSize = 15;

    /// <summary>
    /// The distance in pixels between each line of text.
    /// </summary>
    [FieldOffset(0)]
    public ushort LineHeight;

    /// <summary>
    /// The number of pixels from the absolute top of the line to the base of the characters.
    /// </summary>
    [FieldOffset(2)]
    public ushort Base;

    /// <summary>
    /// The width of the texture, normally used to scale the x pos of the character image.
    /// </summary>
    [FieldOffset(4)]
    public ushort ScaleW;

    /// <summary>
    /// The height of the texture, normally used to scale the y pos of the character image.
    /// </summary>
    [FieldOffset(6)]
    public ushort ScaleH;

    /// <summary>
    /// The number of texture pages included in the font.
    /// </summary>
    [FieldOffset(8)]
    public ushort Pages;

    /// <summary>
    /// The packed bit field. Bits 0-6 are reserved, bit 7 indicates if the monochrome characters have been packed into
    /// each of the texture channels.
    /// </summary>
    [FieldOffset(10)]
    public byte BitField;

    /// <summary>
    /// The alpha channel information.
    /// </summary>
    [FieldOffset(11)]
    public byte AlphaChnl;

    /// <summary>
    /// The red channel information.
    /// </summary>
    [FieldOffset(12)]
    public byte RedChnl;

    /// <summary>
    /// The green channel information.
    /// </summary>
    [FieldOffset(13)]
    public byte GreenChnl;

    /// <summary>
    /// The blue channel information.
    /// </summary>
    [FieldOffset(14)]
    public byte BlueChnl;
}
