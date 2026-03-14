using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Tilemaps.Tiled;

internal static class TiledDataDecoder
{
    private const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
    private const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
    private const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;
    private const uint FLIP_MASK = 0xE0000000;

    public static TilemapTile[,] DecodeTileData(TiledTileLayerDataXml data, int width, int height)
    {
        if (data == null)
        {
            return new TilemapTile[width, height];
        }

        return data.Encoding switch
        {
            null => DecodeXmlData(data.Tiles, width, height),
            "csv" => DecodeCsvData(data.Value, width, height),
            "base64" => DecodeBase64Data(data.Value, data.Compression, width, height),
            _ => throw new TilemapParseException($"Unsupported tile data encoding: '{data.Encoding}'")
        };
    }

    public static TilemapTile[,] DecodeTileData(string value, string encoding, string compression, int width, int height)
    {
        return encoding switch
        {
            "csv" => DecodeCsvData(value, width, height),
            "base64" => DecodeBase64Data(value, compression, width, height),
            _ => throw new TilemapParseException($"Unsupported chunk tile data encoding: '{encoding}'. Chunks support csv and base64 only.")
        };
    }

    private static TilemapTile[,] DecodeXmlData(List<TiledDataTileXml> tiles, int width, int height)
    {
        TilemapTile[,] result = new TilemapTile[width, height];

        if (tiles == null || tiles.Count == 0)
        {
            return result;
        }

        for (int i = 0; i < tiles.Count && i < width * height; i++)
        {
            (int gid, TilemapTileFlipFlags flags) = ExtractFlipFlags(tiles[i].Gid);

            int x = i % width;
            int y = i / width;
            result[x, y] = new TilemapTile(gid, flags);
        }

        return result;
    }

    private static TilemapTile[,] DecodeCsvData(string csv, int width, int height)
    {
        TilemapTile[,] result = new TilemapTile[width, height];

        if (string.IsNullOrWhiteSpace(csv))
        {
            return result;
        }

        string[] values = csv.Split(new[] { ',', '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < values.Length && i < width * height; i++)
        {
            if (!uint.TryParse(values[i].Trim(), out uint rawGid))
            {
                throw new TilemapParseException($"Invalid tile GID in CSV data: '{values[i]}'");
            }

            (int gid, TilemapTileFlipFlags flags) = ExtractFlipFlags(rawGid);

            int x = i % width;
            int y = i / width;
            result[x, y] = new TilemapTile(gid, flags);
        }

        return result;
    }

    private static TilemapTile[,] DecodeBase64Data(string base64, string compression, int width, int height)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return new TilemapTile[width, height];
        }

        byte[] bytes = Convert.FromBase64String(base64.Trim());

        if (!string.IsNullOrEmpty(compression))
        {
            bytes = compression.ToLowerInvariant() switch
            {
                "gzip" => DecompressGzip(bytes),
                "zlib" => DecompressZlib(bytes),
                "zstd" => throw new TilemapParseException(
                    "Tile layer uses Zstandard (zstd) compression, which is not supported. " +
                    "Re-export your map from Tiled using zlib or gzip compression instead " +
                    "(Map > Properties > Tile Layer Format)."),
                _ => throw new TilemapParseException($"Unsupported compression format: '{compression}'")
            };
        }

        TilemapTile[,] result = new TilemapTile[width, height];
        int tileCount = width * height;

        if (bytes.Length < tileCount * 4)
        {
            throw new TilemapParseException($"Insufficient tile data: expected {tileCount * 4} bytes, got {bytes.Length}");
        }

        for (int i = 0; i < tileCount; i++)
        {
            uint rawGid = BitConverter.ToUInt32(bytes, i * 4);
            (int gid, TilemapTileFlipFlags flags) = ExtractFlipFlags(rawGid);

            int x = i % width;
            int y = i / width;
            result[x, y] = new TilemapTile(gid, flags);
        }

        return result;
    }

    private static (int gid, TilemapTileFlipFlags flags) ExtractFlipFlags(uint rawGid)
    {
        // Tiled encodes flip flags in the three most significant bits of the 32-bit GID.
        // The lower 29 bits are the actual tile index. This is a Tiled format constraint,
        // not a design choice: all GID values read from .tmx files use this encoding.
        int gid = (int)(rawGid & ~FLIP_MASK);
        TilemapTileFlipFlags flags = TilemapTileFlipFlags.None;

        if ((rawGid & FLIPPED_HORIZONTALLY_FLAG) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipHorizontally;
        }

        if ((rawGid & FLIPPED_VERTICALLY_FLAG) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipVertically;
        }

        if ((rawGid & FLIPPED_DIAGONALLY_FLAG) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipDiagonally;
        }

        return (gid, flags);
    }

    private static byte[] DecompressGzip(byte[] data)
    {
        try
        {
            using MemoryStream input = new MemoryStream(data);
            using GZipStream gzip = new GZipStream(input, CompressionMode.Decompress);
            using MemoryStream output = new MemoryStream();
            gzip.CopyTo(output);
            return output.ToArray();
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("Failed to decompress gzip data", ex);
        }
    }

    private static byte[] DecompressZlib(byte[] data)
    {
        try
        {
            // Zlib format: 2-byte header + DEFLATE stream + 4-byte Adler-32 checksum.
            if (data.Length < 6)
            {
                throw new TilemapParseException($"Invalid zlib data: too short ({data.Length} bytes)");
            }

            using MemoryStream input = new MemoryStream(data, 2, data.Length - 6);
            using DeflateStream deflate = new DeflateStream(input, CompressionMode.Decompress);
            using MemoryStream output = new MemoryStream();
            deflate.CopyTo(output);
            return output.ToArray();
        }
        catch (Exception ex)
        {
            throw new TilemapParseException("Failed to decompress zlib data", ex);
        }
    }
}
