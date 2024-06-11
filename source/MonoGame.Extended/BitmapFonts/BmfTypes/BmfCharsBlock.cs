// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

[StructLayout(LayoutKind.Explicit)]
internal struct BmfCharsBlock
{
    public const int StructSize = 20;

    [FieldOffset(0)] public uint ID;
    [FieldOffset(4)] public ushort X;
    [FieldOffset(6)] public ushort Y;
    [FieldOffset(8)] public ushort Width;
    [FieldOffset(10)] public ushort Height;
    [FieldOffset(12)] public short XOffset;
    [FieldOffset(14)] public short YOffset;
    [FieldOffset(16)] public short XAdvance;
    [FieldOffset(18)] public byte Page;
    [FieldOffset(19)] public byte Chnl;
}
