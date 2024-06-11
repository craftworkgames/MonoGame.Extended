// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.BitmapFonts.Serialization;

namespace MonoGame.Extended.BitmapFonts.BmfTypes;

internal sealed class BitmapFontFile
{
    public string Path;
    public BmfHeader Header;
    public BmfCommonBlock Common;
    public BmfInfoBlock Info;
    public string FontName;
    public readonly List<string> Pages = new List<string>();
    public readonly List<BmfCharsBlock> Characters = new List<BmfCharsBlock>();
    public readonly List<BmfKerningPairsBlock> Kernings = new List<BmfKerningPairsBlock>();

    public static BitmapFontFile FromStream(FileStream stream)
    {
        using BinaryReader reader = new BinaryReader(stream);

        long position = stream.Position;
        ReadOnlySpan<byte> binaryHeader = stackalloc byte[] { 66, 77, 70, 3 };
        ReadOnlySpan<byte> xmlHeader = stackalloc byte[] { 60, 63, 120, 109 };
        ReadOnlySpan<byte> textHeader = stackalloc byte[] { 105, 110, 102, 111 };

        Span<byte> buffer = stackalloc byte[4];
        stream.Read(buffer);
        stream.Position = position;

        BitmapFontFile bmfFile = new BitmapFontFile();
        bmfFile.Path = stream.Name;


        if (buffer.SequenceEqual(binaryHeader))
        {
            BMFBinaryLoader.Load(bmfFile, stream);
            return bmfFile;
        }

        if (buffer.SequenceEqual(xmlHeader))
        {
            BMFXmlLoader.Load(bmfFile, stream);
            return bmfFile;
        }

        if (buffer.SequenceEqual(textHeader))
        {
            BMFTextLoader.Load(bmfFile, stream);
            return bmfFile;
        }

        throw new InvalidOperationException("This does not appear to be a valid BMFont file!");
    }
}
