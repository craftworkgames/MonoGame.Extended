// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts;

/// <summary>
/// Defines a file reader used to read the contents of a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
public static class BitmapFontBinaryFileReader
{
    /// <summary>
    /// Reads a BMFont binary file from the specified file path.
    /// </summary>
    /// <param name="path">The path to the BMFont binary file.</param>
    /// <returns>A <see cref="BitmapFontFile"/> object containing the data read from the file.</returns>
    public static BitmapFontFile FromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return FromStream(stream);
    }

    /// <summary>
    /// Reads a BMFont binary file from the provided stream.
    /// </summary>
    /// <param name="stream">The stream containing the BMFont binary data.</param>
    /// <returns>A <see cref="BitmapFontFile"/> object containing the data read from the stream.</returns>
    public static BitmapFontFile FromStream(Stream stream)
    {
        using BinaryReader reader = new BinaryReader(stream);

        long position = stream.Position;
        ReadOnlySpan<byte> binaryHeader = stackalloc byte[] { 66, 77, 70, 3 };
        ReadOnlySpan<byte> xmlHeader = stackalloc byte[] { 60, 63, 120, 109 };
        ReadOnlySpan<byte> textHeader = stackalloc byte[] { 105, 110, 102, 111 };

        Span<byte> buffer = stackalloc byte[4];
        stream.Read(buffer);
        stream.Position = position;

        if(buffer.SequenceEqual(binaryHeader))
        {
            return ReadBinary(stream);
        }

        if(buffer.SequenceEqual(xmlHeader))
        {
            return ReadXml(stream);
        }

        if(buffer.SequenceEqual(textHeader))
        {
            return ReadText(stream);
        }

        throw new InvalidOperationException("This does not appear to be a valid BMFont file!");
    }

    /// <summary>
    /// Reads a binary formated BMFont file.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <returns>A <see cref="BitmapFontFile"/> object containing the data read from the stream.</returns>
    private static BitmapFontFile ReadBinary(Stream stream)
    {
        using BinaryReader reader = new BinaryReader(stream);

        BmfHeader header = AsType<BmfHeader>(reader.ReadBytes(BmfHeader.StructSize));
        BmfInfoBlock info = new BmfInfoBlock();
        string fontName = string.Empty;
        BmfCommonBlock common = new BmfCommonBlock();
        string[] pages = Array.Empty<string>();
        BmfCharsBlock[] characters = Array.Empty<BmfCharsBlock>();
        BmfKerningPairsBlock[] kernings = Array.Empty<BmfKerningPairsBlock>();

        if (!header.IsValid)
        {
            throw new InvalidOperationException($"The BMFont file header is invalid, this does not appear to be a valid BMFont file.");
        }

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            byte blockType = reader.ReadByte();
            int blockSize = reader.ReadInt32();

            switch (blockType)
            {
                case 1:
                    info = AsType<BmfInfoBlock>(reader.ReadBytes(BmfInfoBlock.StructSize));
                    int stringLen = blockSize - BmfInfoBlock.StructSize;
                    fontName = Encoding.UTF8.GetString(reader.ReadBytes(stringLen));
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
                    for(int c = 0; c < count; c++)
                    {
                        characters[c] = AsType<BmfCharsBlock>(reader.ReadBytes(BmfCharsBlock.StructSize));
                    }
                    break;

                case 5:
                    int kerningCount = blockSize / BmfKerningPairsBlock.StructSize;
                    kernings = new BmfKerningPairsBlock[kerningCount];
                    for(int k = 0; k < kerningCount; k++)
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

    private static BitmapFontFile ReadXml(Stream stream) { throw new NotImplementedException(); }
    private static BitmapFontFile ReadText(Stream stream) { throw new NotImplementedException(); }

    /// <summary>
    /// Converts a read-only span of bytes to a specified structure type.
    /// </summary>
    /// <typeparam name="T">The type of the structure to convert to.</typeparam>
    /// <param name="buffer">The read-only span of bytes.</param>
    /// <returns>The structure of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an error occurs during conversion.</exception>
    public static T AsType<T>(ReadOnlySpan<byte> buffer) where T : struct
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
