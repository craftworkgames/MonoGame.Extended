// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

[StructLayout(LayoutKind.Explicit)]
internal struct BmfKerningPairsBlock
{
    public const int StructSize = 10;

    [FieldOffset(0)] public uint First;
    [FieldOffset(4)] public uint Second;
    [FieldOffset(8)] public short Amount;
}
