using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Tilemaps.Tiled.Converters;

/// <summary>
/// Converts a parsed <see cref="TiledMapXml"/> document to the format-agnostic
/// <see cref="TilemapData"/> intermediate representation used by both the content pipeline
/// and the runtime parser.
/// </summary>
public static class TiledTilemapDataConverter
{
    private const uint FlipHorizontallyFlag = 0x80000000;
    private const uint FlipVerticallyFlag = 0x40000000;
    private const uint FlipDiagonallyFlag = 0x20000000;
    private const uint FlipMask = 0xE0000000;

    private readonly struct FlipResult
    {
        public FlipResult(int gid, TilemapTileFlipFlags flags)
        {
            Gid = gid;
            Flags = flags;
        }

        public int Gid { get; }
        public TilemapTileFlipFlags Flags { get; }
    }

    /// <summary>
    /// Converts a <see cref="TiledMapXml"/> to a <see cref="TilemapData"/>.
    /// </summary>
    /// <remarks>
    /// External tilesets must be pre-loaded into <see cref="TiledTilesetRefXml.TilesetData"/>
    /// before calling this method if inline data is desired; otherwise they are recorded as
    /// external references.
    /// </remarks>
    /// <param name="map">The parsed Tiled map XML document.</param>
    /// <returns>The format-agnostic tilemap data.</returns>
    /// <exception cref="TilemapParseException">Thrown when the map cannot be converted.</exception>
    public static TilemapData Convert(TiledMapXml map)
    {
        ArgumentNullException.ThrowIfNull(map);

        TilemapData data = new TilemapData();

        data.Width = map.Width;
        data.Height = map.Height;
        data.TileWidth = map.TileWidth;
        data.TileHeight = map.TileHeight;
        data.Orientation = ConvertOrientation(map.Orientation);
        data.StaggerAxis = ConvertStaggerAxis(map.StaggerAxis);
        data.StaggerIndex = ConvertStaggerIndex(map.StaggerIndex);
        data.HexSideLength = map.HexSideLength;
        data.ParallaxOriginX = map.ParallaxOriginX;
        data.ParallaxOriginY = map.ParallaxOriginY;

        if (!string.IsNullOrWhiteSpace(map.BackgroundColor))
        {
            data.BackgroundColor = TiledColorParser.Parse(map.BackgroundColor);
        }

        ConvertProperties(map.Properties, data.Properties);
        ConvertTilesets(map, data);
        ConvertLayers(map.Layers, data.Layers, map);

        return data;
    }

    #region Tilesets

    private static void ConvertTilesets(TiledMapXml map, TilemapData data)
    {
        if (map.Tilesets == null)
        {
            return;
        }

        foreach (TiledTilesetRefXml tilesetRef in map.Tilesets)
        {
            TilemapTilesetEntry entry = new TilemapTilesetEntry();
            entry.FirstGlobalId = tilesetRef.FirstGlobalId;

            // TilesetData is populated when external TSX files have been pre-loaded (runtime path).
            // When it is null and Source is set, this is an unresolved external reference (pipeline path).
            bool isExternalUnresolved = !string.IsNullOrEmpty(tilesetRef.Source)
                                     && tilesetRef.TilesetData == null;

            if (isExternalUnresolved)
            {
                entry.IsExternal = true;
                entry.ExternalPath = tilesetRef.Source;
            }
            else
            {
                TiledTilesetXml tilesetXml = tilesetRef.TilesetData ?? tilesetRef;

                entry.IsExternal = false;
                entry.InlineData = ConvertTilesetData(tilesetXml);
            }

            data.Tilesets.Add(entry);
        }
    }

    internal static TilemapTilesetData ConvertTilesetData(TiledTilesetXml xml)
    {
        TilemapTilesetData data = new TilemapTilesetData();
        data.Name = xml.Name ?? string.Empty;
        data.TexturePath = xml.Image?.Source ?? string.Empty;
        data.TileWidth = xml.TileWidth;
        data.TileHeight = xml.TileHeight;
        data.TileCount = xml.TileCount;
        data.Columns = xml.Columns;
        data.Spacing = xml.Spacing;
        data.Margin = xml.Margin;
        data.DrawOffsetX = xml.TileOffset?.X ?? 0f;
        data.DrawOffsetY = xml.TileOffset?.Y ?? 0f;

        ConvertProperties(xml.Properties, data.Properties);
        ConvertTileEntries(xml.Tiles, data.Tiles);

        return data;
    }

    private static void ConvertTileEntries(List<TiledTileXml> tiledTiles, List<TilemapTileEntryData> entries)
    {
        if (tiledTiles == null || tiledTiles.Count == 0)
        {
            return;
        }

        foreach (TiledTileXml tile in tiledTiles)
        {
            TilemapTileEntryData entry = new TilemapTileEntryData();
            entry.LocalId = tile.Id;
            entry.Class = tile.Class ?? tile.Type ?? string.Empty;
            entry.Probability = tile.Probability <= 0f ? 1.0f : tile.Probability;
            entry.ImagePath = tile.Image?.Source ?? string.Empty;

            ConvertProperties(tile.Properties, entry.Properties);

            if (tile.Animation?.Frames != null && tile.Animation.Frames.Count > 0)
            {
                TilemapAnimationData animation = new TilemapAnimationData();

                foreach (TiledAnimationFrameXml frame in tile.Animation.Frames)
                {
                    // Tiled stores duration in milliseconds; convert to seconds for the runtime.
                    animation.Frames.Add(new TilemapAnimationFrameData
                    {
                        TileId = frame.TileId,
                        Duration = frame.Duration / 1000.0f
                    });
                }

                entry.Animation = animation;
            }

            if (tile.ObjectGroup?.Objects != null)
            {
                foreach (TiledObjectXml obj in tile.ObjectGroup.Objects)
                {
                    TilemapObjectData objData = ConvertObject(obj);

                    if (objData != null)
                    {
                        entry.CollisionObjects.Add(objData);
                    }
                }
            }

            entries.Add(entry);
        }
    }

    #endregion

    #region Layers

    private static void ConvertLayers(List<TiledLayerXml> xmlLayers, List<TilemapLayerData> outputLayers, TiledMapXml map)
    {
        if (xmlLayers == null)
        {
            return;
        }

        foreach (TiledLayerXml layer in xmlLayers)
        {
            TilemapLayerData layerData = ConvertLayer(layer, map);

            if (layerData != null)
            {
                outputLayers.Add(layerData);
            }
        }
    }

    private static TilemapLayerData ConvertLayer(TiledLayerXml layer, TiledMapXml map)
    {
        TilemapLayerData data;

        switch (layer)
        {
            case TiledTileLayerXml tileLayer:
                data = ConvertTileLayer(tileLayer, map);
                break;

            case TiledObjectLayerXml objectLayer:
                data = ConvertObjectLayer(objectLayer);
                break;

            case TiledImageLayerXml imageLayer:
                data = ConvertImageLayer(imageLayer);
                break;

            case TiledGroupLayerXml groupLayer:
                data = ConvertGroupLayer(groupLayer, map);
                break;

            default:
                return null;
        }

        if (data != null)
        {
            ApplyLayerBase(layer, data);
        }

        return data;
    }

    private static void ApplyLayerBase(TiledLayerXml source, TilemapLayerData target)
    {
        target.Name = source.Name ?? string.Empty;
        target.Class = source.Class ?? string.Empty;
        target.IsVisible = source.Visible != 0;
        target.Opacity = source.Opacity;
        target.OffsetX += source.OffsetX;
        target.OffsetY += source.OffsetY;
        target.ParallaxX = source.ParallaxX;
        target.ParallaxY = source.ParallaxY;

        if (!string.IsNullOrWhiteSpace(source.TintColor))
        {
            target.TintColor = TiledColorParser.Parse(source.TintColor);
        }

        ConvertProperties(source.Properties, target.Properties);
    }

    private static TilemapTileLayerData ConvertTileLayer(TiledTileLayerXml tileLayer, TiledMapXml map)
    {
        if (tileLayer.Data == null)
        {
            return new TilemapTileLayerData
            {
                Width = tileLayer.Width > 0 ? tileLayer.Width : map.Width,
                Height = tileLayer.Height > 0 ? tileLayer.Height : map.Height
            };
        }

        bool isChunked = tileLayer.Data.Chunks != null && tileLayer.Data.Chunks.Count > 0;

        if (isChunked)
        {
            return ConvertChunkedTileLayer(tileLayer, map);
        }

        int layerWidth = tileLayer.Width > 0 ? tileLayer.Width : map.Width;
        int layerHeight = tileLayer.Height > 0 ? tileLayer.Height : map.Height;

        TilemapTileLayerData data = new TilemapTileLayerData
        {
            Width = layerWidth,
            Height = layerHeight
        };

        uint[] rawGids = GetRawGids(tileLayer.Data, layerWidth, layerHeight, tileLayer.Name);

        for (int i = 0; i < rawGids.Length; i++)
        {
            uint rawGid = rawGids[i];

            if (rawGid == 0)
            {
                continue;
            }

            FlipResult flip = ExtractFlipFlags(rawGid);
            ushort x = (ushort)(i % layerWidth);
            ushort y = (ushort)(i / layerWidth);
            data.Tiles.Add(new TilemapDecodedTile(x, y, flip.Gid, flip.Flags));
        }

        return data;
    }

    private static TilemapTileLayerData ConvertChunkedTileLayer(TiledTileLayerXml tileLayer, TiledMapXml map)
    {
        // Compute the bounding box of all chunks in tile coordinates.
        int minTileX = int.MaxValue, minTileY = int.MaxValue;
        int maxTileX = int.MinValue, maxTileY = int.MinValue;

        foreach (TiledChunkXml chunk in tileLayer.Data.Chunks)
        {
            if (chunk.X < minTileX)
            {
                minTileX = chunk.X;
            }

            if (chunk.Y < minTileY)
            {
                minTileY = chunk.Y;
            }

            if (chunk.X + chunk.Width > maxTileX)
            {
                maxTileX = chunk.X + chunk.Width;
            }

            if (chunk.Y + chunk.Height > maxTileY)
            {
                maxTileY = chunk.Y + chunk.Height;
            }
        }

        int layerWidth = maxTileX - minTileX;
        int layerHeight = maxTileY - minTileY;

        TilemapTileLayerData data = new TilemapTileLayerData
        {
            Width = layerWidth,
            Height = layerHeight,

            // OffsetX/OffsetY will be combined with the layer's editor offset in ApplyLayerBase.
            // Store the pixel origin so TilemapFactory can apply it as an additional offset.
            OffsetX = minTileX * map.TileWidth,
            OffsetY = minTileY * map.TileHeight
        };

        foreach (TiledChunkXml chunk in tileLayer.Data.Chunks)
        {
            uint[] rawGids = GetRawGids(chunk.Value, tileLayer.Data.Encoding, tileLayer.Data.Compression,
                                        chunk.Width, chunk.Height, tileLayer.Name);

            int baseX = chunk.X - minTileX;
            int baseY = chunk.Y - minTileY;

            for (int i = 0; i < rawGids.Length; i++)
            {
                uint rawGid = rawGids[i];

                if (rawGid == 0)
                {
                    continue;
                }

                FlipResult flip = ExtractFlipFlags(rawGid);
                ushort x = (ushort)(baseX + i % chunk.Width);
                ushort y = (ushort)(baseY + i / chunk.Width);
                data.Tiles.Add(new TilemapDecodedTile(x, y, flip.Gid, flip.Flags));
            }
        }

        return data;
    }

    private static TilemapObjectLayerData ConvertObjectLayer(TiledObjectLayerXml objectLayer)
    {
        TilemapObjectLayerData data = new TilemapObjectLayerData();
        data.DrawOrder = objectLayer.DrawOrder?.ToLowerInvariant() == "index"
                       ? TilemapObjectDrawOrder.Index
                       : TilemapObjectDrawOrder.TopDown;

        if (objectLayer.Objects != null)
        {
            foreach (TiledObjectXml obj in objectLayer.Objects)
            {
                TilemapObjectData objData = ConvertObject(obj);

                if (objData != null)
                {
                    data.Objects.Add(objData);
                }
            }
        }

        return data;
    }

    private static TilemapImageLayerData ConvertImageLayer(TiledImageLayerXml imageLayer)
    {
        if (imageLayer.Image == null || string.IsNullOrWhiteSpace(imageLayer.Image.Source))
        {
            return null;
        }

        return new TilemapImageLayerData
        {
            TexturePath = imageLayer.Image.Source,
            RepeatX = imageLayer.RepeatX != 0,
            RepeatY = imageLayer.RepeatY != 0
        };
    }

    private static TilemapGroupLayerData ConvertGroupLayer(TiledGroupLayerXml groupLayer, TiledMapXml map)
    {
        TilemapGroupLayerData data = new TilemapGroupLayerData();
        ConvertLayers(groupLayer.Layers, data.Layers, map);
        return data;
    }

    #endregion

    #region Objects

    internal static TilemapObjectData ConvertObject(TiledObjectXml obj)
    {
        if (obj == null)
        {
            return null;
        }

        TilemapObjectData data;

        if (obj.Gid > 0)
        {
            FlipResult flip = ExtractFlipFlags((uint)obj.Gid);
            data = new TilemapTileObjectData
            {
                GlobalId = flip.Gid,
                FlipFlags = flip.Flags,
                Width = obj.Width,
                Height = obj.Height
            };
        }
        else if (obj.Text != null)
        {
            data = new TilemapTextObjectData
            {
                Width = obj.Width,
                Height = obj.Height,
                Text = obj.Text.Value ?? string.Empty,
                FontFamily = obj.Text.FontFamily ?? "sans-serif",
                PixelSize = obj.Text.PixelSize > 0 ? obj.Text.PixelSize : 16,
                WordWrap = obj.Text.Wrap != 0,
                Color = TiledColorParser.Parse(obj.Text.Color) ?? Color.Black,
                Bold = obj.Text.Bold != 0,
                Italic = obj.Text.Italic != 0,
                Underline = obj.Text.Underline != 0,
                Strikethrough = obj.Text.Strikeout != 0,
                HorizontalAlign = ConvertHorizontalAlignment(obj.Text.HAlign),
                VerticalAlign = ConvertVerticalAlignment(obj.Text.VAlign)
            };
        }
        else if (obj.Ellipse != null)
        {
            data = new TilemapEllipseObjectData { Width = obj.Width, Height = obj.Height };
        }
        else if (obj.Point != null)
        {
            data = new TilemapPointObjectData();
        }
        else if (obj.Polygon != null)
        {
            TilemapPolygonObjectData polygon = new TilemapPolygonObjectData();
            ParsePolyPoints(obj.Polygon.Points, polygon.Points);
            data = polygon;
        }
        else if (obj.Polyline != null)
        {
            TilemapPolylineObjectData polyline = new TilemapPolylineObjectData();
            ParsePolyPoints(obj.Polyline.Points, polyline.Points);
            data = polyline;
        }
        else
        {
            data = new TilemapRectangleObjectData { Width = obj.Width, Height = obj.Height };
        }

        data.Id = obj.Id;
        data.Name = obj.Name ?? string.Empty;
        data.Class = obj.Class ?? obj.Type ?? string.Empty;
        data.X = obj.X;
        data.Y = obj.Y;

        // Tiled uses degrees; convert to radians for the runtime.
        data.Rotation = obj.Rotation * (MathF.PI / 180.0f);
        data.IsVisible = obj.Visible != 0;

        ConvertProperties(obj.Properties, data.Properties);

        return data;
    }

    #endregion

    #region Properties

    internal static void ConvertProperties(TiledPropertiesXml source, List<TilemapPropertyData> target)
    {
        if (source?.Properties == null || source.Properties.Count == 0)
        {
            return;
        }

        foreach (TiledPropertyXml prop in source.Properties)
        {
            if (string.IsNullOrEmpty(prop.Name))
            {
                continue;
            }

            TilemapPropertyData entry = new TilemapPropertyData();
            entry.Key = prop.Name;
            string type = prop.Type ?? "string";

            switch (type.ToLowerInvariant())
            {
                case "int":
                    entry.Type = TilemapPropertyType.Int;
                    entry.IntValue = int.Parse(prop.Value ?? "0", CultureInfo.InvariantCulture);
                    break;

                case "float":
                    entry.Type = TilemapPropertyType.Float;
                    entry.FloatValue = float.Parse(prop.Value ?? "0", NumberStyles.Float,
                                                   CultureInfo.InvariantCulture);
                    break;

                case "bool":
                    entry.Type = TilemapPropertyType.Bool;
                    entry.BoolValue = bool.Parse(prop.Value ?? "false");
                    break;

                case "color":
                    entry.Type = TilemapPropertyType.Color;
                    entry.ColorValue = TiledColorParser.Parse(prop.Value ?? "#00000000") ?? Color.Transparent;
                    break;

                case "file":
                    entry.Type = TilemapPropertyType.File;
                    entry.StringValue = prop.Value ?? string.Empty;
                    break;

                case "object":
                    entry.Type = TilemapPropertyType.Object;
                    entry.IntValue = int.TryParse(prop.Value, out int objId) ? objId : 0;
                    break;

                default:
                    entry.Type = TilemapPropertyType.String;
                    entry.StringValue = prop.Value ?? string.Empty;
                    break;
            }

            target.Add(entry);
        }
    }

    #endregion

    #region Tile Decoding

    private static uint[] GetRawGids(TiledTileLayerDataXml data, int width, int height, string layerName)
    {
        string encoding = data.Encoding ?? "";

        switch (encoding.ToLowerInvariant())
        {
            case "":
                return DecodeXmlGids(data.Tiles, width, height);

            case "csv":
                return DecodeCsvGids(data.Value, layerName);

            case "base64":
                return DecodeBase64Gids(data.Value, data.Compression ?? "", layerName);

            default:
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' uses unsupported encoding '{encoding}'. " +
                    "Supported encodings are: XML (no encoding), CSV, and Base64.");
        }
    }

    private static uint[] GetRawGids(string rawValue, string encoding, string compression, int width, int height, string layerName)
    {
        switch (encoding?.ToLowerInvariant() ?? "")
        {
            case "csv":
                return DecodeCsvGids(rawValue, layerName);

            case "base64":
                return DecodeBase64Gids(rawValue, compression, layerName);

            default:
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' uses unsupported encoding '{encoding}'. " +
                    "Chunk data requires CSV or Base64 encoding.");
        }
    }

    private static uint[] DecodeXmlGids(List<TiledDataTileXml> tiles, int width, int height)
    {
        uint[] gids = new uint[width * height];

        if (tiles == null || tiles.Count == 0)
        {
            return gids;
        }

        for (int i = 0; i < tiles.Count && i < gids.Length; i++)
        {
            gids[i] = tiles[i].Gid;
        }

        return gids;
    }

    private static uint[] DecodeCsvGids(string csv, string layerName)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return Array.Empty<uint>();
        }

        string[] parts = csv.Split(new[] { ',', '\n', '\r', ' ', '\t' },
                                   StringSplitOptions.RemoveEmptyEntries);
        uint[] gids = new uint[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (!uint.TryParse(parts[i].Trim(), out uint gid))
            {
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' contains an invalid GID value '{parts[i]}' at index {i}.");
            }

            gids[i] = gid;
        }

        return gids;
    }

    private static uint[] DecodeBase64Gids(string base64, string compression, string layerName)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return Array.Empty<uint>();
        }

        byte[] bytes;

        try
        {
            bytes = System.Convert.FromBase64String(base64.Trim());
        }
        catch (FormatException ex)
        {
            throw new TilemapParseException(
                $"Tile layer '{layerName}' contains invalid Base64 tile data.", ex);
        }

        if (!string.IsNullOrEmpty(compression))
        {
            bytes = Decompress(bytes, compression, layerName);
        }

        if (bytes.Length % 4 != 0)
        {
            throw new TilemapParseException(
                $"Tile layer '{layerName}' has {bytes.Length} decoded bytes, which is not a multiple of 4.");
        }

        uint[] gids = new uint[bytes.Length / 4];

        for (int i = 0; i < gids.Length; i++)
        {
            gids[i] = BitConverter.ToUInt32(bytes, i * 4);
        }

        return gids;
    }

    private static byte[] Decompress(byte[] data, string compression, string layerName)
    {
        switch (compression?.ToLowerInvariant())
        {
            case "gzip":
                return DecompressGzip(data, layerName);

            case "zlib":
                return DecompressZlib(data, layerName);

            case "zstd":
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' uses Zstandard (zstd) compression, which is not supported. " +
                    "In Tiled, change the tile layer format to GZip or zlib.");

            default:
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' uses unsupported compression '{compression}'.");
        }
    }

    private static byte[] DecompressGzip(byte[] data, string layerName)
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
            throw new TilemapParseException(
                $"Failed to decompress GZip tile data in layer '{layerName}'.", ex);
        }
    }

    private static byte[] DecompressZlib(byte[] data, string layerName)
    {
        try
        {
            if (data.Length < 6)
            {
                throw new TilemapParseException(
                    $"Tile layer '{layerName}' has ZLib data that is too short ({data.Length} bytes).");
            }

            // Zlib format: 2-byte header + DEFLATE stream + 4-byte Adler-32 checksum.
            using MemoryStream input = new MemoryStream(data, 2, data.Length - 6);
            using DeflateStream deflate = new DeflateStream(input, CompressionMode.Decompress);
            using MemoryStream output = new MemoryStream();
            deflate.CopyTo(output);
            return output.ToArray();
        }
        catch (TilemapParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TilemapParseException(
                $"Failed to decompress ZLib tile data in layer '{layerName}'.", ex);
        }
    }

    #endregion

    #region Parse Helpers

    private static TilemapOrientation ConvertOrientation(string orientation)
    {
        return orientation?.ToLowerInvariant() switch
        {
            "isometric" => TilemapOrientation.Isometric,
            "staggered" => TilemapOrientation.Staggered,
            "hexagonal" => TilemapOrientation.Hexagonal,
            _ => TilemapOrientation.Orthogonal
        };
    }

    private static TilemapStaggerAxis ConvertStaggerAxis(string staggerAxis)
    {
        return staggerAxis?.ToLowerInvariant() == "x"
             ? TilemapStaggerAxis.X
             : TilemapStaggerAxis.Y;
    }

    private static TilemapStaggerIndex ConvertStaggerIndex(string staggerIndex)
    {
        return staggerIndex?.ToLowerInvariant() == "even"
             ? TilemapStaggerIndex.Even
             : TilemapStaggerIndex.Odd;
    }

    private static FlipResult ExtractFlipFlags(uint rawGid)
    {
        int gid = (int)(rawGid & ~FlipMask);
        TilemapTileFlipFlags flags = TilemapTileFlipFlags.None;

        if ((rawGid & FlipHorizontallyFlag) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipHorizontally;
        }

        if ((rawGid & FlipVerticallyFlag) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipVertically;
        }

        if ((rawGid & FlipDiagonallyFlag) != 0)
        {
            flags |= TilemapTileFlipFlags.FlipDiagonally;
        }

        return new FlipResult(gid, flags);
    }

    private static void ParsePolyPoints(string pointsString, List<Vector2> target)
    {
        if (string.IsNullOrWhiteSpace(pointsString))
        {
            return;
        }

        string[] parts = pointsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            string[] xy = part.Split(',');

            if (xy.Length == 2 &&
                float.TryParse(xy[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(xy[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
            {
                target.Add(new Vector2(x, y));
            }
        }
    }

    private static TilemapTextObjectHorizontalAlignment ConvertHorizontalAlignment(string halign)
    {
        return halign?.ToLowerInvariant() switch
        {
            "center" => TilemapTextObjectHorizontalAlignment.Center,
            "right" => TilemapTextObjectHorizontalAlignment.Right,
            "justify" => TilemapTextObjectHorizontalAlignment.Justify,
            _ => TilemapTextObjectHorizontalAlignment.Left
        };
    }

    private static TilemapTextObjectVerticalAlignment ConvertVerticalAlignment(string valign)
    {
        return valign?.ToLowerInvariant() switch
        {
            "center" => TilemapTextObjectVerticalAlignment.Center,
            "bottom" => TilemapTextObjectVerticalAlignment.Bottom,
            _ => TilemapTextObjectVerticalAlignment.Top
        };
    }

    #endregion
}
