using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

[StructLayout(LayoutKind.Explicit)]
internal struct BmfInfoBlock
{
    public const int StructSize = 14;

    [FieldOffset(0)] public short FontSize;
    [FieldOffset(2)] public byte BitField;
    [FieldOffset(3)] public byte CharSet;
    [FieldOffset(4)] public ushort StretchH;
    [FieldOffset(6)] public byte AA;
    [FieldOffset(7)] public byte PaddingUp;
    [FieldOffset(8)] public byte PaddingRight;
    [FieldOffset(9)] public byte PaddingDown;
    [FieldOffset(10)] public byte PaddingLeft;
    [FieldOffset(11)] public byte SpacingHoriz;
    [FieldOffset(12)] public byte SpacingVert;
    [FieldOffset(13)] public byte Outline;
}
