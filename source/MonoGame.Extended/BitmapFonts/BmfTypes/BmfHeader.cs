// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

[StructLayout(LayoutKind.Explicit)]
public struct BmfHeader
{
    public const int StructSize = 4;

    [FieldOffset(0)] public byte B;
    [FieldOffset(1)] public byte M;
    [FieldOffset(2)] public byte F;
    [FieldOffset(3)] public byte Version;

    public readonly bool IsValid => B == 0x42 && M == 0x4D && F == 0x46 && Version == 3;
}
