// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

[StructLayout(LayoutKind.Explicit)]
public struct BmfCommonBlock
{
    public const int StructSize = 15;

    [FieldOffset(0)] public ushort LineHeight;
    [FieldOffset(2)] public ushort Base;
    [FieldOffset(4)] public ushort ScaleW;
    [FieldOffset(6)] public ushort ScaleH;
    [FieldOffset(8)] public ushort Pages;
    [FieldOffset(10)] public byte BitField;
    [FieldOffset(11)] public byte AlphaChnl;
    [FieldOffset(12)] public byte RedChnl;
    [FieldOffset(13)] public byte GreenChnl;
    [FieldOffset(14)] public byte BlueChnl;
}
