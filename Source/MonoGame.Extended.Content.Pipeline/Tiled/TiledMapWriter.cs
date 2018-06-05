using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapContent>
    {
        protected override void Write(ContentWriter writer, TiledMapContent map)
        {
            try
            {
                WriteMetaData(writer, map);
                WriteTilesets(writer, map.Tilesets);
                WriteLayers(writer, map.Layers);
            }
            catch (Exception ex)
            {
                ContentLogger.Logger.LogImportantMessage("Wtf");
                ContentLogger.Logger.LogImportantMessage(ex.StackTrace);
                throw;
            }
        }

        private static void WriteMetaData(ContentWriter writer, TiledMapContent map)
        {
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.TileWidth);
            writer.Write(map.TileHeight);
            writer.Write(ColorHelper.FromHex(map.BackgroundColor));
            writer.Write((byte)map.RenderOrder);
            writer.Write((byte)map.Orientation);
            writer.WriteTiledMapProperties(map.Properties);
        }

        private static void WriteTilesets(ContentWriter writer, IReadOnlyCollection<TiledMapTilesetContent> tilesets)
        {
            writer.Write(tilesets.Count);
            foreach (var tileset in tilesets)
                WriteTileset(writer, tileset);
        }

        private static void WriteTileset(ContentWriter writer, TiledMapTilesetContent tileset)
        {
			writer.WriteObject(tileset.Image.Content);
            writer.Write(tileset.FirstGlobalIdentifier);
            writer.Write(tileset.TileWidth);
            writer.Write(tileset.TileHeight);
            writer.Write(tileset.TileCount);
            writer.Write(tileset.Spacing);
            writer.Write(tileset.Margin);
            writer.Write(tileset.Columns);
            writer.Write(tileset.Tiles.Count);

            foreach (var tilesetTile in tileset.Tiles)
                WriteTilesetTile(writer, tilesetTile);

            writer.WriteTiledMapProperties(tileset.Properties);
        }

        private static void WriteTilesetTile(ContentWriter writer, TiledMapTilesetTileContent tilesetTile)
        {
            writer.Write(tilesetTile.LocalIdentifier);
            writer.Write(tilesetTile.Type);
            writer.Write(tilesetTile.Frames.Count);
            writer.Write(tilesetTile.Objects.Count);

            foreach (var @object in tilesetTile.Objects)
                WriteObject(writer, @object);

            foreach (var frame in tilesetTile.Frames)
            {
                writer.Write(frame.TileIdentifier);
                writer.Write(frame.Duration);
            }

            writer.WriteTiledMapProperties(tilesetTile.Properties);
        }

        private static void WriteLayers(ContentWriter writer, IReadOnlyCollection<TiledMapLayerContent> layers)
        {
            writer.Write(layers.Count);

            foreach (var layer in layers)
                WriteLayer(writer, layer);
        }

        private static void WriteLayer(ContentWriter writer, TiledMapLayerContent layer)
        {
            writer.Write((byte)layer.Type);

            writer.Write(layer.Name ?? string.Empty);
            writer.Write(layer.Visible);
            writer.Write(layer.Opacity);
            writer.Write(layer.OffsetX);
            writer.Write(layer.OffsetY);

            writer.WriteTiledMapProperties(layer.Properties);

            switch (layer.Type)
            {
                case TiledMapLayerType.ImageLayer:
                    WriteImageLayer(writer, (TiledMapImageLayerContent)layer);
                    break;
                case TiledMapLayerType.TileLayer:
                    WriteTileLayer(writer, (TiledMapTileLayerContent)layer);
                    break;
                case TiledMapLayerType.ObjectLayer:
                    WriteObjectLayer(writer, (TiledMapObjectLayerContent)layer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer.Type));
            }
        }

        private static void WriteImageLayer(ContentWriter writer, TiledMapImageLayerContent imageLayer)
        {
            writer.WriteExternalReference(imageLayer.Image.ContentRef);
            writer.Write(new Vector2(imageLayer.X, imageLayer.Y));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WriteTileLayer(ContentWriter writer, TiledMapTileLayerContent tileLayer)
        {
            writer.Write(tileLayer.Width);
            writer.Write(tileLayer.Height);

            writer.Write(tileLayer.Tiles.Length);

            foreach (var tile in tileLayer.Tiles)
            {
                writer.Write(tile.GlobalTileIdentifierWithFlags);
                writer.Write(tile.X);
                writer.Write(tile.Y);
            }
        }

        private static void WriteObjectLayer(ContentWriter writer, TiledMapObjectLayerContent layer)
        {
            writer.Write(ColorHelper.FromHex(layer.Color));
            writer.Write((byte)layer.DrawOrder);

            writer.Write(layer.Objects.Count);

            foreach (var @object in layer.Objects)
                WriteObject(writer, @object);
        }


        private static void WriteObject(ContentWriter writer, TiledMapObjectContent @object)
        {
            var type = GetObjectType(@object);

            writer.Write((byte)type);

            writer.Write(@object.Identifier);
            writer.Write(@object.Name ?? string.Empty);
            writer.Write(@object.Type ?? string.Empty);
            writer.Write(@object.X);
            writer.Write(@object.Y);
            writer.Write(@object.Width);
            writer.Write(@object.Height);
            writer.Write(@object.Rotation);
            writer.Write(@object.Visible);

            writer.WriteTiledMapProperties(@object.Properties);

            switch (type)
            {
                case TiledMapObjectType.Rectangle:
                case TiledMapObjectType.Ellipse:
                    break;
                case TiledMapObjectType.Tile:
                    writer.Write(@object.GlobalIdentifier);
                    break;
                case TiledMapObjectType.Polygon:
                    WritePolyPoints(writer, @object.Polygon.Points);
                    break;
                case TiledMapObjectType.Polyline:
                    WritePolyPoints(writer, @object.Polyline.Points);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void WritePolyPoints(ContentWriter writer, string @string)
        {
            var stringPoints = @string.Split(' ');

            writer.Write(stringPoints.Length);

            foreach (var stringPoint in stringPoints)
            {
                var xy = stringPoint.Split(',');
                var x = float.Parse(xy[0], CultureInfo.InvariantCulture.NumberFormat);
                writer.Write(x);
                var y = float.Parse(xy[1], CultureInfo.InvariantCulture.NumberFormat);
                writer.Write(y);
            }
        }

        public static TiledMapObjectType GetObjectType(TiledMapObjectContent content)
        {
            if (content.GlobalIdentifier > 0)
                return TiledMapObjectType.Tile;

            if (content.Ellipse != null)
                return TiledMapObjectType.Ellipse;

            if (content.Polygon != null)
                return TiledMapObjectType.Polygon;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (content.Polyline != null)
                return TiledMapObjectType.Polyline;

            return TiledMapObjectType.Rectangle;
        }
        
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Tiled.TiledMap, MonoGame.Extended.Tiled";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Tiled.TiledMapReader, MonoGame.Extended.Tiled";
        }
    }
}