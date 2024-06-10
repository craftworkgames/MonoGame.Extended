// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts;

internal class BMFBinaryLoader
{
    public static BitmapFontFile Load(Stream stream)
    {
        using BinaryReader reader = new BinaryReader(stream);
        BmfHeader header = LoadHeader(reader);
        BmfInfoBlock info = default;
        BmfCommonBlock common = default;
        BmfCharsBlock[] characters = Array.Empty<BmfCharsBlock>();
        BmfKerningPairsBlock[] kernings = Array.Empty<BmfKerningPairsBlock>();
        string[] pages = Array.Empty<string>();
        string fontName = string.Empty;

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            byte blockType = reader.ReadByte();
            int blockSize = reader.ReadInt32();

            switch (blockType)
            {
                case 1:
                    info = AsType<BmfInfoBlock>(reader.ReadBytes(BmfInfoBlock.StructSize));
                    int stringLen = blockSize - BmfInfoBlock.StructSize;
                    fontName = Encoding.UTF8.GetString(reader.ReadBytes(stringLen)).Replace("\0", string.Empty);
                    break;

                case 2:
                    common = AsType<BmfCommonBlock>(reader.ReadBytes(BmfCommonBlock.StructSize));
                    break;

                case 3:
                    pages = Encoding.UTF8.GetString(reader.ReadBytes(blockSize)).Split('\0', StringSplitOptions.RemoveEmptyEntries);
                    break;

                case 4:
                    int count = blockSize / BmfCharsBlock.StructSize;
                    characters = new BmfCharsBlock[count];
                    for (int c = 0; c < count; c++)
                    {
                        characters[c] = AsType<BmfCharsBlock>(reader.ReadBytes(BmfCharsBlock.StructSize));
                    }
                    break;

                case 5:
                    int kerningCount = blockSize / BmfKerningPairsBlock.StructSize;
                    kernings = new BmfKerningPairsBlock[kerningCount];
                    for (int k = 0; k < kerningCount; k++)
                    {
                        kernings[k] = AsType<BmfKerningPairsBlock>(reader.ReadBytes(BmfKerningPairsBlock.StructSize));
                    }
                    break;

                default:
                    reader.BaseStream.Seek(blockSize, SeekOrigin.Current);
                    break;
            }
        }

        BitmapFontFile file = new BitmapFontFile(header, common, info, fontName, pages, characters, kernings);
        return file;
    }

    private static BmfHeader LoadHeader(BinaryReader reader)
    {
        BmfHeader header = AsType<BmfHeader>(reader.ReadBytes(BmfHeader.StructSize));
        if (!header.IsValid)
        {
            throw new InvalidOperationException($"The BMFont file header is invalid, this does not appear to be a valid BMFont file.");
        }
        return header;
    }

    private static T AsType<T>(ReadOnlySpan<byte> buffer) where T : struct
    {
        T value;
        try
        {
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    value = Marshal.PtrToStructure<T>((IntPtr)ptr);
                }
            }
            return value;
        }
        catch
        {
            return default;
        }
    }
}
