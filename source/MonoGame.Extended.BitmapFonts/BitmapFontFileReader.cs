// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.BitmapFonts;

/// <summary>
/// Defines a file reader used to read the contents of a BMFont binary file.
/// </summary>
/// <seealso href="https://www.angelcode.com/products/bmfont/doc/file_format.html#bin"/>
public static class BitmapFontFileReader
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

        if (buffer.SequenceEqual(binaryHeader))
        {
            return BMFBinaryLoader.Load(stream);
        }

        if (buffer.SequenceEqual(xmlHeader))
        {
            return BMFXmlLoader.Load(stream);
        }

        if (buffer.SequenceEqual(textHeader))
        {
            return BMFTextLoader.LoadText(stream);
        }

        throw new InvalidOperationException("This does not appear to be a valid BMFont file!");
    }
}
