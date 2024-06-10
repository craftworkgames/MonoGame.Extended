// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.


using System;
using System.Formats.Asn1;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;
using NSubstitute;
using NuGet.Frameworks;

namespace MonoGame.Extended.BitmapFonts.Tests;

public class BitmapFontFileReaderTests
{
    private readonly BitmapFontFile _expected;

    public BitmapFontFileReaderTests()
    {
        _expected = CreateExpected();
    }

    private static BitmapFontFile CreateExpected()
    {
        BmfHeader header;
        header.B = (byte)'B';
        header.M = (byte)'M';
        header.F = (byte)'F';
        header.Version = 3;

        BmfInfoBlock info;
        info.FontSize = 32;
        //  bitField
        //  bit 0: smooth
        //  bit 1: unicode,
        //  bit 2: italic,
        //  bit 3: bold,
        //  bit 4: fixedHeigth,
        //  bits 5 - 7: reserved
        info.BitField = 0b1100_0000;
        info.CharSet = 0;
        info.StretchH = 50;
        info.AA = 1;
        info.PaddingUp = 1;
        info.PaddingRight = 2;
        info.PaddingDown = 3;
        info.PaddingLeft = 4;
        info.SpacingHoriz = 6;
        info.SpacingVert = 5;
        info.Outline = 2;

        string fontName = "Cute Dino";

        BmfCommonBlock common;
        common.LineHeight = 16;
        common.Base = 12;
        common.ScaleW = 256;
        common.ScaleH = 256;
        common.Pages = 1;
        common.BitField = 0b0000_0000;
        common.AlphaChnl = 1;
        common.RedChnl = 0;
        common.GreenChnl = 0;
        common.BlueChnl = 0;

        string[] pages = new string[] { "test-font_0.png" };

        BmfCharsBlock char0;
        char0.ID = 70;
        char0.X = 34;
        char0.Y = 0;
        char0.Width = 27;
        char0.Height = 20;
        char0.XOffset = -5;
        char0.YOffset = -3;
        char0.XAdvance = 17;
        char0.Page = 0;
        char0.Chnl = 15;

        BmfCharsBlock char1;
        char1.ID = 74;
        char1.X = 0;
        char1.Y = 0;
        char1.Width = 28;
        char1.Height = 20;
        char1.XOffset = -6;
        char1.YOffset = -3;
        char1.XAdvance = 18;
        char1.Page = 0;
        char1.Chnl = 15;
        BmfCharsBlock[] characters = new BmfCharsBlock[] { char0, char1 };

        BmfKerningPairsBlock kerning0;
        kerning0.First = 70;
        kerning0.Second = 74;
        kerning0.Amount = -1;
        BmfKerningPairsBlock[] kernings = new BmfKerningPairsBlock[] { kerning0 };

        return new BitmapFontFile(header, common, info, fontName, pages, characters, kernings);
    }

    [Fact]
    public void Read_BinaryFile_Test()
    {
        BitmapFontFile actual = BitmapFontFileReader.FromFile("files/bmfont/test-font-binary.fnt");
        Assert.Equal(_expected.Header, actual.Header);
        Assert.Equal(_expected.Info, actual.Info);
        Assert.Equal(_expected.Common, actual.Common);
        Assert.Equal(_expected.FontName, actual.FontName);
        Assert.True(_expected.Pages.SequenceEqual(actual.Pages));
        Assert.True(_expected.Characters.SequenceEqual(actual.Characters));
        Assert.True(_expected.Kernings.SequenceEqual(actual.Kernings));
    }

    [Fact]
    public void Read_XmlFile_Test()
    {
        BitmapFontFile actual = BitmapFontFileReader.FromFile("files/bmfont/test-font-xml.fnt");
        Assert.Equal(_expected.Header, actual.Header);
        Assert.Equal(_expected.Info, actual.Info);
        Assert.Equal(_expected.Common, actual.Common);
        Assert.Equal(_expected.FontName, actual.FontName);
        Assert.True(_expected.Pages.SequenceEqual(actual.Pages));
        Assert.True(_expected.Characters.SequenceEqual(actual.Characters));
        Assert.True(_expected.Kernings.SequenceEqual(actual.Kernings));
    }

    [Fact]
    public void Read_Text_Test()
    {
        BitmapFontFile actual = BitmapFontFileReader.FromFile("files/bmfont/test-font-text.fnt");
        Assert.Equal(_expected.Header, actual.Header);
        Assert.Equal(_expected.Info, actual.Info);
        Assert.Equal(_expected.Common, actual.Common);
        Assert.Equal(_expected.FontName, actual.FontName);
        Assert.True(_expected.Pages.SequenceEqual(actual.Pages));
        Assert.True(_expected.Characters.SequenceEqual(actual.Characters));
        Assert.True(_expected.Kernings.SequenceEqual(actual.Kernings));
    }

    private static void AssertEqualHeader(BmfHeader expected, BmfHeader actual)
    {
        Assert.Equal(expected.B, actual.B);
        Assert.Equal(expected.M, actual.M);
        Assert.Equal(expected.F, actual.F);
        Assert.Equal(expected.Version, actual.Version);
    }

    private static void AssertEqualInfo(BmfInfoBlock expected, BmfInfoBlock actual)
    {
        Assert.Equal(expected.FontSize, actual.FontSize);
        Assert.Equal(expected.BitField, actual.BitField);
        Assert.Equal(expected.CharSet, actual.CharSet);
        Assert.Equal(expected.StretchH, actual.StretchH);
        Assert.Equal(expected.AA, actual.AA);
        Assert.Equal(expected.PaddingUp, actual.PaddingUp);
        Assert.Equal(expected.PaddingRight, actual.PaddingRight);
        Assert.Equal(expected.PaddingDown, actual.PaddingDown);
        Assert.Equal(expected.PaddingLeft, actual.PaddingLeft);
        Assert.Equal(expected.SpacingHoriz, actual.SpacingHoriz);
        Assert.Equal(expected.SpacingVert, actual.SpacingVert);
        Assert.Equal(expected.Outline, actual.Outline);
    }

    private static void AssertEqual(BitmapFontFile expected, BitmapFontFile actual)
    {
        Assert.Equal(expected.Header, actual.Header);
        Assert.Equal(expected.Info, actual.Info);
        Assert.Equal(expected.FontName, actual.FontName);
        Assert.Equal(expected.Common, actual.Common);
        Assert.True(expected.Characters.SequenceEqual(actual.Characters));
        Assert.True(expected.Kernings.SequenceEqual(actual.Kernings));
    }
}
