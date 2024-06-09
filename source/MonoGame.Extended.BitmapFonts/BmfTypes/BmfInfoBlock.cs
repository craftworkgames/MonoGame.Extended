using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

/// <summary>
/// Defines the structure of the info block in a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
[StructLayout(LayoutKind.Explicit)]
public struct BmfInfoBlock
{
    /// <summary>
    /// The size of the BmfInfoBlock structure in bytes.
    /// </summary>
    public const int StructSize = 14;

    /// <summary>
    /// The font size.
    /// </summary>
    [FieldOffset(0)]
    public short FontSize;

    /// <summary>
    /// Bit field that holds various flags.
    /// </summary>
    /// <remarks>
    /// - Bit 0: Smooth
    /// - Bit 1: Unicode
    /// - Bit 2: Italic
    /// - Bit 3: Bold
    /// - Bit 4: Fixed Height
    /// - Bits 5-7: Reserved
    /// </remarks>
    [FieldOffset(2)]
    public byte BitField;

    /// <summary>
    /// The character set used.
    /// </summary>
    [FieldOffset(3)]
    public byte CharSet;

    /// <summary>
    /// The stretch height percentage.
    /// </summary>
    [FieldOffset(4)]
    public ushort StretchH;

    /// <summary>
    /// The anti-aliasing level.
    /// </summary>
    [FieldOffset(6)]
    public byte AA;

    /// <summary>
    /// The padding at the top of the characters.
    /// </summary>
    [FieldOffset(7)]
    public byte PaddingUp;

    /// <summary>
    /// The padding to the right of the characters.
    /// </summary>
    [FieldOffset(8)]
    public byte PaddingRight;

    /// <summary>
    /// The padding at the bottom of the characters.
    /// </summary>
    [FieldOffset(9)]
    public byte PaddingDown;

    /// <summary>
    /// The padding to the left of the characters.
    /// </summary>
    [FieldOffset(10)]
    public byte PaddingLeft;

    /// <summary>
    /// The horizontal spacing between characters.
    /// </summary>
    [FieldOffset(11)]
    public byte SpacingHoriz;

    /// <summary>
    /// The vertical spacing between characters.
    /// </summary>
    [FieldOffset(12)]
    public byte SpacingVert;

    /// <summary>
    /// The outline thickness.
    /// </summary>
    [FieldOffset(13)]
    public byte Outline;
}
