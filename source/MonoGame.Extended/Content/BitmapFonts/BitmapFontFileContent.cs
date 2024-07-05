// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Content.BitmapFonts;

public sealed class BitmapFontFileContent
{
    public string Path;
    public HeaderBlock Header;
    public CommonBlock Common;
    public InfoBlock Info;
    public string FontName;
    public readonly List<string> Pages = new List<string>();
    public readonly List<CharacterBlock> Characters = new List<CharacterBlock>();
    public readonly List<KerningPairsBlock> Kernings = new List<KerningPairsBlock>();

    #region BitmapFont File Content Block Definitions

    [StructLayout(LayoutKind.Explicit)]
    public struct HeaderBlock
    {
        public const int StructSize = 4;

        [FieldOffset(0)] public byte B;
        [FieldOffset(1)] public byte M;
        [FieldOffset(2)] public byte F;
        [FieldOffset(3)] public byte Version;

        public readonly bool IsValid => B == 0x42 && M == 0x4D && F == 0x46 && Version == 3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InfoBlock
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

    [StructLayout(LayoutKind.Explicit)]
    public struct CharacterBlock
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


    [StructLayout(LayoutKind.Explicit)]
    public struct CommonBlock
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


    [StructLayout(LayoutKind.Explicit)]
    public struct KerningPairsBlock
    {
        public const int StructSize = 10;

        [FieldOffset(0)] public uint First;
        [FieldOffset(4)] public uint Second;
        [FieldOffset(8)] public short Amount;
    }

    #endregion BitmapFont File Content Block Definitions
}
