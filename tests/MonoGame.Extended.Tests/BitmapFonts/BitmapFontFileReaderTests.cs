// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.


using System;
using System.IO;
using System.Linq;
using MonoGame.Extended.Content.BitmapFonts;

namespace MonoGame.Extended.BitmapFonts.Tests;

public class BitmapFontFileReaderTests
{
    private readonly BitmapFontFileContent _expected;

    public BitmapFontFileReaderTests()
    {
        _expected = CreateExpected();
    }

    private static BitmapFontFileContent CreateExpected()
    {
        BitmapFontFileContent bmfFile = new BitmapFontFileContent()
        {
            Header = new()
            {
                B = (byte)'B',
                M = (byte)'M',
                F = (byte)'F',
                Version = 3
            },
            Info = new()
            {
                FontSize = 32,
                BitField = 0b1100_0000,
                CharSet = 0,
                StretchH = 50,
                AA = 1,
                PaddingUp = 1,
                PaddingRight = 2,
                PaddingDown = 3,
                PaddingLeft = 4,
                SpacingHoriz = 6,
                SpacingVert = 5,
                Outline = 2
            },
            FontName = "Cute Dino",
            Common = new()
            {
                LineHeight = 16,
                Base = 12,
                ScaleW = 256,
                ScaleH = 256,
                Pages = 1,
                BitField = 0b0000_0000,
                AlphaChnl = 1,
                RedChnl = 0,
                GreenChnl = 0,
                BlueChnl = 0,
            }
        };

        bmfFile.Pages.Add("test-font_0.png");
        bmfFile.Characters.Add(new()
        {
            ID = 70,
            X = 34,
            Y = 0,
            Width = 27,
            Height = 20,
            XOffset = -5,
            YOffset = -3,
            XAdvance = 17,
            Page = 0,
            Chnl = 15
        });
        bmfFile.Characters.Add(new()
        {
            ID = 74,
            X = 0,
            Y = 0,
            Width = 28,
            Height = 20,
            XOffset = -6,
            YOffset = -3,
            XAdvance = 18,
            Page = 0,
            Chnl = 15,
        });

        bmfFile.Kernings.Add(new()
        {
            First = 70,
            Second = 74,
            Amount = -1
        });

        return bmfFile;
    }

    [Fact]
    public void Read_BinaryFile_Test()
    {
        string path = "BitmapFonts/files/bmfont/test-font-binary.fnt";
        using FileStream stream = File.OpenRead(path);
        var actual = BitmapFontFileReader.Read(stream, path);
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
        string path = "BitmapFonts/files/bmfont/test-font-xml.fnt";
        using FileStream stream = File.OpenRead(path);
        var actual = BitmapFontFileReader.Read(stream, path);
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
        string path = "BitmapFonts/files/bmfont/test-font-text.fnt";
        using FileStream stream = File.OpenRead(path);
        var actual = BitmapFontFileReader.Read(stream, path);
        Assert.Equal(_expected.Header, actual.Header);
        Assert.Equal(_expected.Info, actual.Info);
        Assert.Equal(_expected.Common, actual.Common);
        Assert.Equal(_expected.FontName, actual.FontName);
        Assert.True(_expected.Pages.SequenceEqual(actual.Pages));
        Assert.True(_expected.Characters.SequenceEqual(actual.Characters));
        Assert.True(_expected.Kernings.SequenceEqual(actual.Kernings));
    }
}
