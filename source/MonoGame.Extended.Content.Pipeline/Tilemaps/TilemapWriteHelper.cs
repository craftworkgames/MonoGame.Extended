using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Byte constants used to identify layer types in the binary format.
/// </summary>
internal static class TilemapLayerTypeByte
{
    internal const byte Tile = 0;
    internal const byte Object = 1;
    internal const byte Image = 2;
    internal const byte Group = 3;
    internal const byte Data = 4;
}

/// <summary>
/// Byte constants used to identify object types in the binary format.
/// </summary>
internal static class TilemapObjectTypeByte
{
    internal const byte Rectangle = 0;
    internal const byte Ellipse = 1;
    internal const byte Point = 2;
    internal const byte Polygon = 3;
    internal const byte Polyline = 4;
    internal const byte Tile = 5;
    internal const byte Text = 6;
}

/// <summary>
/// Byte constants used to identify property value types in the binary format.
/// </summary>
internal static class TilemapPropertyTypeByte
{
    internal const byte String = 0;
    internal const byte Int = 1;
    internal const byte Float = 2;
    internal const byte Bool = 3;
    internal const byte Color = 4;
    internal const byte File = 5;
}

/// <summary>
/// Shared serialization helpers for the Tilemap content pipeline writers.
/// </summary>
internal static class TilemapWriteHelper
{
    internal static void WriteMap(ContentWriter writer, TilemapData map, IExternalReferenceRepository refs)
    {
        WriteMapHeader(writer, map);
        WriteTilesets(writer, map, refs);
        WriteLayers(writer, map.Layers, refs);
    }

    private static void WriteMapHeader(ContentWriter writer, TilemapData map)
    {
        writer.Write(map.Width);
        writer.Write(map.Height);
        writer.Write(map.TileWidth);
        writer.Write(map.TileHeight);
        writer.Write((byte)map.Orientation);
        writer.Write((byte)map.StaggerAxis);
        writer.Write((byte)map.StaggerIndex);
        writer.Write(map.HexSideLength);
        WriteOptionalColor(writer, map.BackgroundColor);
        writer.Write(map.ParallaxOriginX);
        writer.Write(map.ParallaxOriginY);
        writer.Write(map.WorldX);
        writer.Write(map.WorldY);
        writer.Write(map.WorldDepth);
        WriteProperties(writer, map.Properties);
    }

    private static void WriteTilesets(ContentWriter writer, TilemapData map, IExternalReferenceRepository refs)
    {
        writer.Write(map.Tilesets.Count);

        foreach (TilemapTilesetEntry entry in map.Tilesets)
        {
            writer.Write(entry.FirstGlobalId);
            writer.Write(entry.IsExternal);

            if (entry.IsExternal)
            {
                ExternalReference<TilemapTilesetData> tilesetRef =
                    refs.GetExternalReference<TilemapTilesetData>(entry.ExternalPath);
                writer.WriteExternalReference(tilesetRef);
            }
            else
            {
                TilemapTilesetWriter.WriteTileset(writer, entry.InlineData, refs);
            }
        }
    }

    private static void WriteLayers(ContentWriter writer, List<TilemapLayerData> layers, IExternalReferenceRepository refs)
    {
        writer.Write(layers.Count);

        foreach (TilemapLayerData layer in layers)
        {
            WriteLayer(writer, layer, refs);
        }
    }

    private static void WriteLayer(ContentWriter writer, TilemapLayerData layer, IExternalReferenceRepository refs)
    {
        switch (layer)
        {
            case TilemapTileLayerData tileLayer:
                writer.Write(TilemapLayerTypeByte.Tile);
                WriteLayerBase(writer, layer);
                WriteTileLayer(writer, tileLayer);
                break;

            case TilemapObjectLayerData objectLayer:
                writer.Write(TilemapLayerTypeByte.Object);
                WriteLayerBase(writer, layer);
                WriteObjectLayer(writer, objectLayer);
                break;

            case TilemapImageLayerData imageLayer:
                writer.Write(TilemapLayerTypeByte.Image);
                WriteLayerBase(writer, layer);
                WriteImageLayer(writer, imageLayer, refs);
                break;

            case TilemapGroupLayerData groupLayer:
                writer.Write(TilemapLayerTypeByte.Group);
                WriteLayerBase(writer, layer);
                WriteLayers(writer, groupLayer.Layers, refs);
                break;

            case TilemapDataLayerData dataLayer:
                writer.Write(TilemapLayerTypeByte.Data);
                WriteLayerBase(writer, layer);
                WriteDataLayer(writer, dataLayer);
                break;

            default:
                throw new InvalidContentException(
                    $"Unsupported layer data type '{layer.GetType().Name}' for layer '{layer.Name}'. " +
                    "This is an internal error; please report it.");
        }
    }

    private static void WriteLayerBase(ContentWriter writer, TilemapLayerData layer)
    {
        writer.Write(layer.Name ?? string.Empty);
        writer.Write(layer.Class ?? string.Empty);
        writer.Write(layer.IsVisible);
        writer.Write(layer.Opacity);
        WriteOptionalColor(writer, layer.TintColor);
        writer.Write(layer.OffsetX);
        writer.Write(layer.OffsetY);
        writer.Write(layer.ParallaxX);
        writer.Write(layer.ParallaxY);
        WriteProperties(writer, layer.Properties);
    }

    private static void WriteTileLayer(ContentWriter writer, TilemapTileLayerData tileLayer)
    {
        writer.Write(tileLayer.Width);
        writer.Write(tileLayer.Height);
        writer.Write(tileLayer.Tiles.Count);

        foreach (TilemapDecodedTile tile in tileLayer.Tiles)
        {
            writer.Write(tile.X);
            writer.Write(tile.Y);
            writer.Write(tile.GlobalId);
            writer.Write((byte)tile.FlipFlags);
        }
    }

    private static void WriteObjectLayer(ContentWriter writer, TilemapObjectLayerData objectLayer)
    {
        writer.Write(objectLayer.DrawOrder == TilemapObjectDrawOrder.Index ? (byte)1 : (byte)0);
        writer.Write(objectLayer.Objects.Count);

        foreach (TilemapObjectData obj in objectLayer.Objects)
        {
            WriteObject(writer, obj);
        }
    }

    private static void WriteImageLayer(ContentWriter writer, TilemapImageLayerData imageLayer, IExternalReferenceRepository refs)
    {
        ExternalReference<Texture2DContent> textureRef = refs.GetExternalReference<Texture2DContent>(imageLayer.TexturePath ?? string.Empty);
        writer.WriteExternalReference(textureRef);

        // Image layer position is managed through the layer offset (OffsetX, OffsetY).
        writer.Write(imageLayer.RepeatX);
        writer.Write(imageLayer.RepeatY);
    }

    private static void WriteDataLayer(ContentWriter writer, TilemapDataLayerData dataLayer)
    {
        writer.Write(dataLayer.Width);
        writer.Write(dataLayer.Height);
    }

    internal static void WriteProperties(ContentWriter writer, IReadOnlyList<TilemapPropertyData> properties)
    {
        if (properties == null || properties.Count == 0)
        {
            writer.Write(0);
            return;
        }

        writer.Write(properties.Count);

        foreach (TilemapPropertyData property in properties)
        {
            WriteProperty(writer, property);
        }
    }

    private static void WriteProperty(ContentWriter writer, TilemapPropertyData property)
    {
        writer.Write(property.Key ?? string.Empty);

        switch (property.Type)
        {
            case TilemapPropertyType.Int:
                writer.Write(TilemapPropertyTypeByte.Int);
                writer.Write(property.IntValue);
                break;
            case TilemapPropertyType.Float:
                writer.Write(TilemapPropertyTypeByte.Float);
                writer.Write(property.FloatValue);
                break;
            case TilemapPropertyType.Bool:
                writer.Write(TilemapPropertyTypeByte.Bool);
                writer.Write(property.BoolValue);
                break;
            case TilemapPropertyType.Color:
                writer.Write(TilemapPropertyTypeByte.Color);
                WriteColor(writer, property.ColorValue);
                break;
            case TilemapPropertyType.File:
                writer.Write(TilemapPropertyTypeByte.File);
                writer.Write(property.StringValue ?? string.Empty);
                break;
            default:
                // String, Object, and any unrecognized types are serialized as string.
                writer.Write(TilemapPropertyTypeByte.String);
                writer.Write(property.StringValue ?? string.Empty);
                break;
        }
    }

    internal static void WriteTileEntries(ContentWriter writer, IReadOnlyList<TilemapTileEntryData> tiles, IExternalReferenceRepository refs)
    {
        if (tiles == null || tiles.Count == 0)
        {
            writer.Write(0);
            return;
        }

        writer.Write(tiles.Count);

        foreach (TilemapTileEntryData tile in tiles)
        {
            WriteTileEntry(writer, tile, refs);
        }
    }

    private static void WriteTileEntry(ContentWriter writer, TilemapTileEntryData tile, IExternalReferenceRepository refs)
    {
        writer.Write(tile.LocalId);
        writer.Write(tile.Class ?? string.Empty);
        writer.Write(tile.Probability <= 0f ? 1.0f : tile.Probability);
        WriteProperties(writer, tile.Properties);
        WriteAnimation(writer, tile.Animation);
        WriteCollisionObjects(writer, tile.CollisionObjects);

        bool hasTileImage = !string.IsNullOrEmpty(tile.ImagePath);
        writer.Write(hasTileImage);

        if (hasTileImage)
        {
            ExternalReference<Texture2DContent> tileTexRef =
                refs.GetExternalReference<Texture2DContent>(tile.ImagePath);
            writer.WriteExternalReference(tileTexRef);
        }
    }

    private static void WriteAnimation(ContentWriter writer, TilemapAnimationData animation)
    {
        if (animation == null || animation.Frames == null || animation.Frames.Count == 0)
        {
            writer.Write(false);
            return;
        }

        writer.Write(true);
        writer.Write(animation.Frames.Count);

        foreach (TilemapAnimationFrameData frame in animation.Frames)
        {
            writer.Write(frame.TileId);
            writer.Write(frame.Duration);
        }
    }

    private static void WriteCollisionObjects(ContentWriter writer, IReadOnlyList<TilemapObjectData> objects)
    {
        if (objects == null || objects.Count == 0)
        {
            writer.Write(0);
            return;
        }

        writer.Write(objects.Count);

        foreach (TilemapObjectData obj in objects)
        {
            WriteObject(writer, obj);
        }
    }

    internal static void WriteObject(ContentWriter writer, TilemapObjectData obj)
    {
        byte objectType = GetObjectTypeByte(obj);
        writer.Write(objectType);
        writer.Write(obj.Id);
        writer.Write(obj.Name ?? string.Empty);
        writer.Write(obj.Class ?? string.Empty);
        writer.Write(obj.X);
        writer.Write(obj.Y);
        writer.Write(obj.Rotation);
        writer.Write(obj.IsVisible);
        WriteProperties(writer, obj.Properties);

        switch (objectType)
        {
            case TilemapObjectTypeByte.Rectangle:
                TilemapRectangleObjectData rect = (TilemapRectangleObjectData)obj;
                writer.Write(rect.Width);
                writer.Write(rect.Height);
                break;
            case TilemapObjectTypeByte.Ellipse:
                TilemapEllipseObjectData ellipse = (TilemapEllipseObjectData)obj;
                writer.Write(ellipse.Width);
                writer.Write(ellipse.Height);
                break;
            case TilemapObjectTypeByte.Point:
                break;
            case TilemapObjectTypeByte.Polygon:
                WritePolyPoints(writer, ((TilemapPolygonObjectData)obj).Points);
                break;
            case TilemapObjectTypeByte.Polyline:
                WritePolyPoints(writer, ((TilemapPolylineObjectData)obj).Points);
                break;
            case TilemapObjectTypeByte.Tile:
                TilemapTileObjectData tile = (TilemapTileObjectData)obj;
                writer.Write(tile.GlobalId);
                writer.Write((byte)tile.FlipFlags);
                writer.Write(tile.Width);
                writer.Write(tile.Height);
                break;
            case TilemapObjectTypeByte.Text:
                WriteTextObject(writer, (TilemapTextObjectData)obj);
                break;
        }
    }

    private static void WriteTextObject(ContentWriter writer, TilemapTextObjectData obj)
    {
        writer.Write(obj.Width);
        writer.Write(obj.Height);
        writer.Write(obj.Text ?? string.Empty);
        writer.Write(obj.FontFamily ?? "sans-serif");
        writer.Write(obj.PixelSize);
        writer.Write(obj.WordWrap);
        WriteColor(writer, obj.Color);
        writer.Write(obj.Bold);
        writer.Write(obj.Italic);
        writer.Write(obj.Underline);
        writer.Write(obj.Strikethrough);
        writer.Write((byte)obj.HorizontalAlign);
        writer.Write((byte)obj.VerticalAlign);
    }

    private static void WritePolyPoints(ContentWriter writer, IReadOnlyList<Vector2> points)
    {
        if (points == null || points.Count == 0)
        {
            writer.Write(0);
            return;
        }

        writer.Write(points.Count);

        foreach (Vector2 point in points)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }
    }

    internal static void WriteOptionalColor(ContentWriter writer, Color? color)
    {
        if (!color.HasValue)
        {
            writer.Write(false);
            return;
        }

        writer.Write(true);
        WriteColor(writer, color.Value);
    }

    internal static void WriteColor(ContentWriter writer, Color color)
    {
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
        writer.Write(color.A);
    }

    private static byte GetObjectTypeByte(TilemapObjectData obj)
    {
        return obj switch
        {
            TilemapTileObjectData => TilemapObjectTypeByte.Tile,
            TilemapEllipseObjectData => TilemapObjectTypeByte.Ellipse,
            TilemapPointObjectData => TilemapObjectTypeByte.Point,
            TilemapPolygonObjectData => TilemapObjectTypeByte.Polygon,
            TilemapPolylineObjectData => TilemapObjectTypeByte.Polyline,
            TilemapTextObjectData => TilemapObjectTypeByte.Text,
            _ => TilemapObjectTypeByte.Rectangle
        };
    }
}
