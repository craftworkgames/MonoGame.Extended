// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts.Serialization;

internal class BMFBinaryLoader
{
    public static void Load(BitmapFontFile bmfFile, Stream stream)
    {
        using BinaryReader reader = new BinaryReader(stream);
        bmfFile.Header = LoadHeader(reader);
        BmfCharsBlock[] characters = Array.Empty<BmfCharsBlock>();
        BmfKerningPairsBlock[] kernings = Array.Empty<BmfKerningPairsBlock>();

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            byte blockType = reader.ReadByte();
            int blockSize = reader.ReadInt32();

            switch (blockType)
            {
                case 1:
                    bmfFile.Info = AsType<BmfInfoBlock>(reader.ReadBytes(BmfInfoBlock.StructSize));
                    int stringLen = blockSize - BmfInfoBlock.StructSize;
                    bmfFile.FontName = Encoding.UTF8.GetString(reader.ReadBytes(stringLen)).Replace("\0", string.Empty);
                    break;

                case 2:
                    bmfFile.Common = AsType<BmfCommonBlock>(reader.ReadBytes(BmfCommonBlock.StructSize));
                    break;

                case 3:
                    string[] pages = Encoding.UTF8.GetString(reader.ReadBytes(blockSize)).Split('\0', StringSplitOptions.RemoveEmptyEntries);
                    bmfFile.Pages.AddRange(pages);
                    break;

                case 4:
                    int count = blockSize / BmfCharsBlock.StructSize;
                    for (int c = 0; c < count; c++)
                    {
                        BmfCharsBlock character = AsType<BmfCharsBlock>(reader.ReadBytes(BmfCharsBlock.StructSize));
                        bmfFile.Characters.Add(character);
                    }
                    break;

                case 5:
                    int kerningCount = blockSize / BmfKerningPairsBlock.StructSize;
                    kernings = new BmfKerningPairsBlock[kerningCount];
                    for (int k = 0; k < kerningCount; k++)
                    {
                        BmfKerningPairsBlock kerning = AsType<BmfKerningPairsBlock>(reader.ReadBytes(BmfKerningPairsBlock.StructSize));
                        bmfFile.Kernings.Add(kerning);
                    }
                    break;

                default:
                    reader.BaseStream.Seek(blockSize, SeekOrigin.Current);
                    break;
            }
        }
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
